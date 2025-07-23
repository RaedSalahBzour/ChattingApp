using AutoMapper;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattingAppAPI.Controllers;
[Authorize]
public class MessageController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
    {
        var username = User.GetUserName();
        if (username == messageDto.RecipientUsername.ToLower())
            return BadRequest("can not message your self");

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);
        if (sender == null || recipient == null)
            return BadRequest("cannot send message at this time");
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            Content = messageDto.Content,
            SenderUsername = username,
            RecipientUsername = messageDto.RecipientUsername,
        };
        unitOfWork.MessageRepository.AddMessage(message);
        if (await unitOfWork.Complete()) return mapper.Map<MessageDto>(message);
        return BadRequest("faild to save");
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser
        ([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();

        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);
        return Ok(messages);
    }
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUserName();
        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var message = await unitOfWork.MessageRepository.GetMessage(id);
        if (message is null) return BadRequest("cannot delete this message");
        if (message.SenderUsername != username && message.RecipientUsername != username)
            return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;
        if (message is { SenderDeleted: true, RecipientDeleted: true })
            unitOfWork.MessageRepository.DeleteMessage(message);
        if (await unitOfWork.Complete())
            return Ok();
        return BadRequest("problem occured when deleting the message");
    }
}
