using MemoApp.Common;
using MemoApp.Data;

namespace MemoApp.Services
{
    public interface IMemoService
    {
        IFeedback<long> AddMemo(Memo memo);
        IResult<Memo> GetMemoById(long id);
        IResult<Memo> UpdateMemo(Memo memo);
        IFeedback<bool> DeleteMemo(long id);
    }
}
