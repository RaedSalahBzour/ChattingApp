using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;

namespace ChattingAppAPI.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo?> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
