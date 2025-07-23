using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingAppAPI.DTOs;
using ChattingAppAPI.Entities;
using ChattingAppAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChattingAppAPI.Data.Repositories;

public class PhotoRepository(ApplicationDbContext context, IMapper mapper) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        var photos = await context.Photos.IgnoreQueryFilters().Where(p => p.IsApproved == false)
            .ProjectTo<PhotoForApprovalDto>(mapper.ConfigurationProvider).ToListAsync();
        return photos;
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
