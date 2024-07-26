using DevToys.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.SimpleSequenceExecutor.UI;
public interface IUIExecutor : IUICard
{
    bool CanExecute { get; }
    ValueTask ExecuteAsync();
}
