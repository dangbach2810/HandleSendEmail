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
        public static void Main(string[] args)
        {
            DataContextDataContext _context = new DataContextDataContext();
            var listUserAcceptMail = _context.SettingEmails.ToList();
            var users = _context.Users.ToList();
            var tasks = _context.Tasks.ToList();

            foreach (var item in listUserAcceptMail)
            {
                var user = users.Where(u => u.Id == item.Id).FirstOrDefault();
                Console.WriteLine(user.Email);
                Console.WriteLine(user.Lastname + " " + user.Firstname);
                var userTasks = tasks.Where(t => t.CreatedBy == item.Id);
                if (item.SendEmail == true)
                {
                    if (item.SendDaily == true)
                    {
                        var countTaskSuccessByDay = userTasks.Where(t => t.IsActive == true).Count(t => t.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                        var countTaskOnGoingByDay = userTasks.Where(t => t.IsActive == false).Count(t => t.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                        Console.WriteLine(countTaskSuccessByDay);
                        Console.WriteLine(countTaskOnGoingByDay);
                    }
                    if (item.SendWeekly == true && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        var countTaskSuccessByWeek = userTasks.Where(t => t.IsActive == true).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                        var countTaskOnGoingByWeek = userTasks.Where(t => t.IsActive == false).GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.CreatedOn.GetValueOrDefault().Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    }
                    if (item.SendMonthly == true && DateTime.Now.Day == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                    {
                        var countTaskSuccessByMonth = userTasks.Where(t => t.IsActive == true).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Date.Month });
                        var countTaskOnGoingByMonth = userTasks.Where(t => t.IsActive == false).GroupBy(t => new { t.CreatedOn.GetValueOrDefault().Year, t.CreatedOn.GetValueOrDefault().Month });
                    }
                    if (item.SendYearly == true && DateTime.Now.DayOfYear == 365)
                    {
                        var countTaskSuccessByYear = userTasks.Where(t => t.IsActive == true).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year);
                        var countTaskOnGoingByYear = userTasks.Where(t => !t.IsActive == false).GroupBy(t => t.CreatedOn.GetValueOrDefault().Year);
                    }

                }
                else
                {
                    Console.WriteLine("Nguoi dung nay khong nhan Email");
                }
            }
            Console.ReadLine();
        }
        
    }
}
