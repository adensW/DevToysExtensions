using Adens.DevToys.SimpleSequenceExecutor.Args;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutor : IUICard
{
    public event EventHandler? ParametersChanged;
    Dictionary<string, object> Parameters { get; set; }
    bool CanExecute { get; }
    ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables);
}
