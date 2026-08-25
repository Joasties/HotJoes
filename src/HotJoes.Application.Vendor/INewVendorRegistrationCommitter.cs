namespace HotJoes.Application.Vendor;

public interface INewVendorRegistrationCommitter
{
    Task CommitAsync(
        NewVendorRegistrationCommit commit,
        CancellationToken cancellationToken);
}
