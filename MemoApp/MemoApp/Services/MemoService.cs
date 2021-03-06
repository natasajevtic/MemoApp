﻿using MemoApp.Common;
using MemoApp.Constants;
using MemoApp.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
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

                memo.CreatedAt = DateTime.UtcNow;
                memo.UpdatedAt = memo.CreatedAt;
                memo.Status = _entities.Statuses.Where(s => s.Name == Statuses.Active.ToString()).FirstOrDefault();

                _entities.Memos.Add(memo);

                _entities.SaveChanges();
                feedback.Value = memo.Id;
                feedback.Status = StatusEnum.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add a new memo: {ex.Message}");
                feedback.Status = StatusEnum.Error;
            }
            return feedback;
        }

        public IResult<Memo> GetMemoById(long id)
        {
            var result = new Result<Memo>();
            try
            {
                result.Value = _entities.Memos.Where(m => m.Id == id && m.Status.Name == Statuses.Active.ToString())
                    .Include(m => m.Tags)
                    .Include(m => m.Status)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get memo by id {id}: {ex.Message}");
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
                    memoToUpdate.UpdatedAt = DateTime.UtcNow;
                    memoToUpdate.Note = memo.Note;
                    memoToUpdate.Title = memo.Title;

                    var existingTags = memoToUpdate.Tags.Select(t => t.Name).OrderBy(t => t);
                    var newTags = memo.Tags.Select(t => t.Name).OrderBy(t => t);

                    //checking if collections do not contain the same tag names; whether tags are changed
                    if (!existingTags.SequenceEqual(newTags))
                    {
                        _entities.Tags.RemoveRange(memoToUpdate.Tags);
                        _entities.Tags.AddRange(memo.Tags);
                    }

                    _entities.SaveChanges();
                    result.Value = memoToUpdate;
                    result.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update a memo by id {memo.Id}: {ex.Message}");
            }
            return result;
        }

        public IFeedback<bool> DeleteMemo(long id)
        {
            var feedback = new Feedback<bool>();
            try
            {
                var memoToDelete = GetMemoById(id).Value;
                if (memoToDelete != null)
                {
                    memoToDelete.Status = _entities.Statuses.Where(s => s.Name == Statuses.Deleted.ToString()).FirstOrDefault();
                    _entities.SaveChanges();
                    feedback.Value = true;
                    feedback.Status = StatusEnum.Succeeded;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to delete a memo by id {id}: {ex.Message}");
                feedback.Status = StatusEnum.Error;
            }
            return feedback;
        }

        public IResult<List<Memo>> GetAllMemos()
        {
            var result = new Result<List<Memo>>();
            try
            {
                result.Value = _entities.Memos.Where(m => m.Status.Name == Statuses.Active.ToString())
                    .Include(m => m.Tags)
                    .Include(m => m.Status)
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get all memos: {ex.Message}");
            }
            return result;
        }

        public IResult<List<Memo>> GetUserMemos(string userId)
        {
            var result = new Result<List<Memo>>();
            try
            {
                result.Value = _entities.Memos.Where(m => m.Status.Name == Statuses.Active.ToString() && m.UserId == userId)
                    .Include(m => m.Tags)
                    .Include(m => m.Status)
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user {userId} memos: {ex.Message}");
            }
            return result;
        }

        public IResult<Memo> GetUserMemoById(string userId, long memoId)
        {
            var result = new Result<Memo>();
            try
            {
                result.Value = _entities.Memos.Where(m => m.Id == memoId && m.UserId == userId && m.Status.Name == Statuses.Active.ToString())
                    .Include(m => m.Tags)
                    .Include(m => m.Status)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get user {userId} memo by id {memoId}: {ex.Message}");
            }
            return result;
        }

        public IFeedback<long> AddSettings(Setting settings)
        {
            var feedback = new Feedback<long>();
            try
            {
                _entities.Settings.Add(settings);

                _entities.SaveChanges();
                feedback.Value = settings.Id;
                feedback.Status = StatusEnum.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add a new settings: {ex.Message}");
                feedback.Status = StatusEnum.Error;
            }
            return feedback;
        }

        public IResult<Setting> UpdateSettings(Setting settings)
        {
            var result = new Result<Setting>();
            try
            {
                var settingsToUpdate = GetSettingsById(settings.Id).Value;
                if (settingsToUpdate != null)
                {
                    settingsToUpdate.ZoneName = settings.ZoneName;
                    settingsToUpdate.DateFormat = settings.DateFormat;
                    settingsToUpdate.TimeFormat = settings.TimeFormat;
                    settingsToUpdate.Culture = settings.Culture;
                    _entities.SaveChanges();
                    result.Value = settingsToUpdate;
                    result.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update a settings by id {settings.Id}: {ex.Message}");
            }
            return result;
        }

        public IResult<Setting> GetSettingsById(long id)
        {
            var result = new Result<Setting>();
            try
            {
                result.Value = _entities.Settings.Where(s => s.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get settings by id {id}: {ex.Message}");
            }
            return result;
        }
    }
}
