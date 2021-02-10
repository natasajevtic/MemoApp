using MemoApp.Common;
using MemoApp.Data;

namespace MemoApp.Services
{
    public interface IMemoService
    {
        IFeedback<long> AddMemo(Memo memo);        
    }
}
