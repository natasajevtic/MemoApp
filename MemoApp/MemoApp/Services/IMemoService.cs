using MemoApp.Common;
using MemoApp.Data;
using System.Collections.Generic;

namespace MemoApp.Services
{
    public interface IMemoService
    {
        IFeedback<long> AddMemo(Memo memo);
        IResult<Memo> GetMemoById(long id);
        IResult<List<Memo>> GetMemos();
        IResult<Memo> UpdateMemo(Memo memo);
        IFeedback<bool> DeleteMemo(long id);
    }
}
