using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IWireCell
{
    public int WireCount { get; }
    public int OutputCount { get; }
    public List<bool> ActiveStates { get; }
    public Action<IWireCell> ChangedRotation { get; set; }
}
