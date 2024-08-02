using Xunit;

namespace Adens.DevToys.Testbase;

[CollectionDefinition(nameof(TestParallelizationDisabled), DisableParallelization = true)]
public class TestParallelizationDisabled
{
}
