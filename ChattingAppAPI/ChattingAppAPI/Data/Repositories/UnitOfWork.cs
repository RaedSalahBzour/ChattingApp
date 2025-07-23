using ChattingAppAPI.Interfaces;

namespace ChattingAppAPI.Data.Repositories;

public class UnitOfWork(ApplicationDbContext context, ILikeRepository likeRepository
    , IUserRepository userRepository, IMessageRepository messageRepository
    , IPhotoRepository photoRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;
    public IMessageRepository MessageRepository => messageRepository;
    public ILikeRepository LikeRepository => likeRepository;
    public IPhotoRepository PhotoRepository => photoRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
