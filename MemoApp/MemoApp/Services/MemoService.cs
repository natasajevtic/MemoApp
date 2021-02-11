using MemoApp.Common;
using MemoApp.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;

namespace MemoApp.Services
{
    public class MemoService : IMemoService
    {
        private readonly MemoEntities _entities;

        public MemoService(MemoEntities entities)
        {
            _entities = entities;
        }

        public IFeedback<long> AddMemo(Memo memo)
        {
            var feedback = new Feedback<long>();
            try
            {
                _entities.Tags.AddRange(memo.Tags);

                memo.CreatedAt = DateTime.Now;
                memo.UpdatedAt = memo.CreatedAt;
                memo.Status = _entities.Statuses.Where(s => s.Name == "Active").FirstOrDefault();

                _entities.Memos.Add(memo);

                if (Commit().Value)
                {
                    feedback.Value = memo.Id;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add a new memo: {ex.GetBaseException().Message}");
            }
            return feedback;
        }        

        public IResult<bool> Commit()
        {
            var result = new Result<bool>();
            try
            {
                result.Value = _entities.SaveChanges() > 0;
            }
            catch (Exception ex)
            {                
                Log.Error($"Failed to save changes to database: {ex.GetBaseException().Message}");
            }
            return result;
        }

        public IResult<Memo> GetMemoById(long id)
        {
            var result = new Result<Memo>();
            try
            {
                result.Value = _entities.Memos.Where(m => m.Id == id)
                    .Include(m => m.Tags)
                    .Include(m => m.Status)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get memo by id {id}: {ex.GetBaseException().Message}");
            }
            return result;
        }

        public IResult<Memo> UpdateMemo(Memo memo)
        {
            var result = new Result<Memo>();
            try
            {
                var memoToUpdate = GetMemoById(memo.Id).Value;
                if (memoToUpdate != null)
                {
                    memoToUpdate.UpdatedAt = DateTime.Now;
                    memoToUpdate.Note = memo.Note;
                    memoToUpdate.Title = memo.Title;
                    
                    _entities.Tags.RemoveRange(memoToUpdate.Tags);
                    _entities.Tags.AddRange(memo.Tags);

                    if (Commit().Value)
                    {
                        result.Value = memoToUpdate;
                        result.Succeeded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update a memo by id {memo.Id}: {ex.GetBaseException().Message}");
            }
            return result;
        }
    }
}
