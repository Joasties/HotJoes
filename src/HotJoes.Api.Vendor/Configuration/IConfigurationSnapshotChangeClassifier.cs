namespace HotJoes.Api.Vendor.Configuration;

public interface IConfigurationSnapshotChangeClassifier<TOptions>
    where TOptions : class
{
    ConfigurationChangeClassification Classify(
        TOptions currentSnapshot,
        TOptions candidateSnapshot);
}
