using System;
using System.IO;
using System.Text;
using System.Web;

namespace OrderMangement.Helpers
{

    public static class ErrorLogger
    {
        /// <summary>
        /// 記錄錯誤到 error_log.txt
        /// </summary>
        /// <param name="ex">例外物件</param>
        public static void LogError(Exception ex)
        {
            try
            {
                // 錯誤訊息
                var logMessage = new StringBuilder();
                logMessage.AppendLine($"========== 錯誤記錄 ==========");
                logMessage.AppendLine($"時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logMessage.AppendLine($"錯誤訊息: {ex.Message}");

                // 紀錄堆疊
                logMessage.AppendLine("堆疊追蹤:");
                logMessage.AppendLine(ex.StackTrace ?? "無堆疊資訊");
                logMessage.AppendLine(new string('=', 100));
                logMessage.AppendLine();

                string logPath = HttpContext.Current.Server.MapPath("~/error_log.txt");

                // 寫入檔案（附加模式）
                File.AppendAllText(logPath, logMessage.ToString(), Encoding.UTF8);
            }
            catch (Exception logEx)
            {
                // 如果記錄錯誤時發生問題，寫入 Windows 事件記錄
                System.Diagnostics.EventLog.WriteEntry(
                    "Application",
                    $"ErrorLogger 失敗: {logEx.Message}\n原始錯誤: {ex.Message}",
                    System.Diagnostics.EventLogEntryType.Error
                );
            }
        }
    }
}
