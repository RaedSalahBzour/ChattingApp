using AutoMapper;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Extensions;
using ChattingAppAPI.Helpers;
using ChattingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace ChattingAppAPI.Controllers;

public class MessageController(IMessageRepository messageRepository,
    IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto messageDto)
    {
        var username = User.GetUserName();
        if (username == messageDto.RecipientUsername.ToLower())
            return BadRequest("can not message your self");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);
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
        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync()) return mapper.Map<MessageDto>(message);
        return BadRequest("faild to save");
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser
        ([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);
        return Ok(messages);
    }
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUserName();
        return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }
}
