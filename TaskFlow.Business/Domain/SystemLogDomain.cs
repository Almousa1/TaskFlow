using TaskFlow.Business.Domain.Common;
using TaskFlow.Business.ViewModels;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TaskFlow.Business.Domain
{
    public enum LogActionType { Insert, Update, Delete }

    public class SystemLogDomain : BaseDomain
    {
        private readonly SystemLogRepository _systemLogRepository;
        private readonly SystemUserRepository _systemUserRepository;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic),
            WriteIndented = false,
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public SystemLogDomain()
        {
            _systemLogRepository = new SystemLogRepository(_context);
            _systemUserRepository = new SystemUserRepository(_context);
        }

        public async Task<int> LogAsync(
            LogActionType action,
            string tableName,
            int affectedRecord,
            int userId = 0,
            object oldValues = null,
            object newValues = null)
        {
            static string ToJson(object data)
            {
                return JsonSerializer.Serialize(data, JsonOptions);
            }

            var log = new tblSystemLog
            {
                guid = Guid.NewGuid(),
                Action = action.ToString(),
                TableName = tableName,
                AffectedRecord = affectedRecord,
                UserId = userId,
                OldValues = ToJson(oldValues),
                NewValues = ToJson(newValues),
                Timestamp = DateTime.Now,
            };

            return await _systemLogRepository.InsertSystemLog(log);
        }

        public async Task<List<SystemLogViewModel>> GetSystemLog(DateTime? startDate, DateTime? endDate, string email)
        {
            int? userId = await _systemUserRepository.FindKfuIdByEmailAsync(email);
            var logs = await _systemLogRepository.GetSystemLogs(startDate, endDate, userId);

            return logs.Select(x => new SystemLogViewModel
            {
                guid = x.guid,
                Id = x.Id,
                Name = x.Name,
                NameAr = x.NameAr,
                UserId = x.UserId,
                StudentId = x.StudentId,
                Action = x.Action,
                TableName = x.TableName,
                AffectedRecord = x.AffectedRecord,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                Timestamp = x.Timestamp
            }).OrderByDescending(x => x.Timestamp).ToList();
        }

        public async Task<SystemLogViewModel> GetLogDetailsAsync(Guid guid)
        {
            var log = await _systemLogRepository.GetSystemLogByGuid(guid);
            if (log == null) return null;

            static string Pretty(string json)
            {
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    return JsonSerializer.Serialize(doc, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic),
                        WriteIndented = true
                    });
                }
                catch
                {
                    return json;
                }
            }

            return new SystemLogViewModel
            {
                guid = log.guid,
                Id = log.Id,
                Name = log.Name,
                NameAr = log.NameAr,
                UserId = log.UserId,
                StudentId = log.StudentId,
                Action = log.Action,
                TableName = log.TableName,
                AffectedRecord = log.AffectedRecord,
                OldValues = Pretty(log.OldValues),
                NewValues = Pretty(log.NewValues),
                Timestamp = log.Timestamp
            };
        }
    }
}
