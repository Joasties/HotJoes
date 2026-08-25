namespace HotJoes.Application.Vendor;

public abstract class RegistrationOutcomeDetermination
{
    private RegistrationOutcomeDetermination()
    {
    }

    public static RegistrationOutcomeDetermination FirstProcessingRequired()
    {
        return new FirstProcessing();
    }

    public static RegistrationOutcomeDetermination Replay(
        RegisterVendorResult.Success originalResult)
    {
        ArgumentNullException.ThrowIfNull(originalResult);

        return new EquivalentReplay(originalResult);
    }

    public static RegistrationOutcomeDetermination ConflictDetected()
    {
        return new Conflict();
    }

    public sealed class FirstProcessing : RegistrationOutcomeDetermination
    {
        internal FirstProcessing()
        {
        }
    }

    public sealed class EquivalentReplay : RegistrationOutcomeDetermination
    {
        internal EquivalentReplay(RegisterVendorResult.Success originalResult)
        {
            OriginalResult = originalResult;
        }

        public RegisterVendorResult.Success OriginalResult { get; }
    }

    public sealed class Conflict : RegistrationOutcomeDetermination
    {
        internal Conflict()
        {
        }
    }
}
