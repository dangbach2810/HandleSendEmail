using MailUtil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleSendEmail
{
    public class Program
    {
        public static void FormEmail(string _emailUser, string time,int totalProject, int totalCard, int totalDoneTask, int totalDoingTask)
        {
            SendEmail sendEmail = new SendEmail();
            StringBuilder content = new StringBuilder("");
            content.Append("<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Email Content</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n            border: 2px solid #ccc;\r\n            border-radius: 10px;\r\n            max-width: 600px;\r\n            margin: auto;\r\n            padding: 20px;\r\n        }\r\n        h2 {\r\n            text-align: center;\r\n            color: #333;\r\n            border: 2px black solid;\r\n        }\r\n        p {\r\n\r\n            margin: 10px 0;\r\n        }\r\n        p strong {\r\n            font-weight: bold;\r\n            color: #007bff;\r\n        }\r\n    </style>\r\n</head>");//header
            content.Append("<body>\r\n    <h2>Số liệu dưới đây được thống kê theo "+time+"</h2>");//Title
            content.Append("<p><strong>Tổng số dự án đã tạo trong "+time+" qua:</strong> "+totalProject+"</p>");//Project
            content.Append("<p><strong>Tổng số thẻ đã tạo trong "+time+" này:</strong> "+totalCard+"</p>");//Card
            content.Append("<p><strong>Hoàn thành được nhiệm vụ:</strong> "+totalDoneTask+"</p>");//TaskDone
            content.Append("<p><strong>Tổng nhiệm vụ vẫn đang làm:</strong> "+totalDoingTask+"</p>");//Doing Task
            content.Append("<i>Cảm ơn bạn đã sử dụng hệ thống.Chúng tôi sẽ tiếp tục phát triển và cải thiện hệ thống để cung cấp những giải pháp\r\n        tốt nhất cho cộng đồng người dùng.\r\n        Nếu có bất kỳ thắc mắc hoặc góp ý nào, xin đừng ngần ngại liên hệ với tôi.\r\n    </i>");//Thanks
            sendEmail.SendMailGoogleSmtp("locdangbach@gmail.com", _emailUser, "Thống kê từ Hệ thống quản lý kế hoạch cá nhân", content.ToString(), "locdangbach@gmail.com", "oguh rpmo inqr adkg");
        }
        public static void Main(string[] args)
        {
            DataContextDataContext _context = new DataContextDataContext();
            var listUserAcceptMail = _context.SettingEmails.ToList();
            var users = _context.Users.ToList();
            var tasks = _context.Tasks.ToList();
            var cards = _context.Cards.ToList();
            var projects = _context.Projects.ToList();
            string time = "";
            foreach (var item in listUserAcceptMail)
            {
                var user = users.Where(u => u.Id == item.UserId).FirstOrDefault();
                if(user != null)
                {
                    var userTasks = tasks.Where(t => t.CreatedBy == item.UserId);
                    if (item.SendEmail == true)
                    {
                        if (item.SendDaily == true)
                        {
                            time = "Ngày";
                            var countProjectByDay = projects.Where(p => p.CreatedBy == item.UserId).Count(c => c.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                            var countCardByDay = cards.Where(c => c.CreatedBy == item.UserId).Count(c => c.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                            var countTaskSuccessByDay = userTasks.Where(t => t.IsActive == true ).Count(t => t.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                            var countTaskOnGoingByDay = userTasks.Where(t => t.IsActive == false ).Count(t => t.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                            FormEmail(user.Email, time, countProjectByDay, countCardByDay, countTaskSuccessByDay, countTaskOnGoingByDay);
                        }
                        if (item.SendWeekly == true && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            time = "Tuần";
                            var countProjectByWeek = projects.Where(t => t.IsActive == true).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).ToList().Count;
                            var countCardByWeek = cards.Where(t => t.IsActive == true).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).ToList().Count;
                            var countTaskSuccessByWeek = userTasks.Where(t => t.IsActive == true ).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).ToList().Count;
                            var countTaskOnGoingByWeek = userTasks.Where(t => t.IsActive == false).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)).ToList().Count;
                            FormEmail(user.Email, time, countProjectByWeek, countCardByWeek,
                                countTaskSuccessByWeek, countTaskOnGoingByWeek);
                        }
                        if (item.SendMonthly == true && DateTime.Now.Day == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                        {
                            time = "Tháng";
                            var countProjectByMonth = projects.Where(t => t.IsActive == true).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Date.Month }).ToList().Count;
                            var countCardByMonth = cards.Where(t => t.IsActive == true).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Date.Month }).ToList().Count;
                            var countTaskSuccessByMonth = userTasks.Where(t => t.IsActive == true).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Date.Month }).ToList().Count;
                            var countTaskOnGoingByMonth = userTasks.Where(t => t.IsActive == false).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Month }).ToList().Count;
                            FormEmail(user.Email, time, countProjectByMonth, countCardByMonth,
                                      countTaskSuccessByMonth, countTaskOnGoingByMonth);
                        }
                        if (item.SendYearly == true && DateTime.Now.DayOfYear == 365)
                        {
                            time = "Năm";
                            var countProjectByYear = projects.Where(t => t.IsActive == true && t.CreatedBy == item.UserId).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year).ToList().Count;
                            var countCardByYear = cards.Where(t => t.IsActive == true && t.CreatedBy == item.UserId).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year).ToList().Count;
                            var countTaskSuccessByYear = userTasks.Where(t => t.IsActive == true).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year).ToList().Count;
                            var countTaskOnGoingByYear = userTasks.Where(t => !t.IsActive == false).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year).ToList().Count;
                            FormEmail(user.Email, time, countProjectByYear, countCardByYear,
                                          countTaskSuccessByYear, countTaskOnGoingByYear);
                        }

                    }
                }
            }
        }
        
    }
}

