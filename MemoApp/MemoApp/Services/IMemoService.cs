using MemoApp.Common;
using MemoApp.Data;
using System.Collections.Generic;

namespace MemoApp.Services
{
    public interface IMemoService
    {
        IFeedback<long> AddMemo(Memo memo);
        IResult<Memo> GetMemoById(long id);
        IResult<Memo> GetUserMemoById(string userId, long memoId);
        IResult<List<Memo>> GetAllMemos();
        IResult<List<Memo>> GetUserMemos(string userId);
        IResult<Memo> UpdateMemo(Memo memo);
        IFeedback<bool> DeleteMemo(long id);

        IFeedback<long> AddSettings(Setting settings);
        IResult<Setting> UpdateSettings(Setting settings);
        IResult<Setting> GetSettingsById(long id);
    }
}
