using Adens.DevToys.SimpleSequenceExecutor.Args;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutor : IUICard
{
     event EventHandler? ParametersChanged;
     ObservableCollection<BundleStepParameter> Parameters { get; set; }
    bool CanExecute { get; }
    ValueTask<ExecutedResult> ExecuteAsync(Dictionary<string, object> runtimeVariables);
}
