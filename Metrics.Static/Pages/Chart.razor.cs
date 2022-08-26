using Metrics.Core;
using Metrics.Static.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Reflection;

namespace Metrics.Static.Pages
{
    public class ChartBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }

        [Parameter]
        public int OffSet { get; set; }

        [Parameter]
        public MetricType Type { get; set; } = 0;

        [Parameter]
        public string Username { get; set; } = "funkysi1701";

        protected string Title;
        protected bool LoadCompleteH = false;
        protected bool LoadCompleteD = false;
        protected bool LoadCompleteM = false;

        protected List<DateTime> hourlyLabel = new();
        protected List<DateTime> dailyLabel = new();
        protected List<DateTime> monthlyLabel = new();

        protected List<decimal> hourlyData = new();
        protected List<decimal> dailyData = new();
        protected List<decimal> monthlyData = new();

        protected List<decimal> hourlyPrevData = new();
        protected List<decimal> dailyPrevData = new();
        protected List<decimal> monthlyPrevData = new();

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(Username))
            {
                Username = "funkysi1701";
            }
            Load();
        }

        public void RefreshMe()
        {
            StateHasChanged();
        }

        protected async Task LoadHourly(string Username)
        {
            IList<IList<ChartView>> hourlyChart = await BlogService.GetChart((int)Type, (int)MyChartType.Hourly, OffSet, Username);
            foreach (var subitem in hourlyChart[0].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyData.Add(subitem.Total.Value);
                }
            }

            foreach (var subitem in hourlyChart[1].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyPrevData.Add(subitem.Total.Value);
                }
            }
            if (hourlyData.Count > 0)
            {
                LoadCompleteH = true;
            }
            StateHasChanged();
        }

        protected async Task LoadDaily(string Username)
        {
            IList<IList<ChartView>> dailyChart = await BlogService.GetChart((int)Type, (int)MyChartType.Daily, OffSet, Username);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            LoadCompleteD = true;
            StateHasChanged();
        }

        protected async Task LoadMonthly(string Username)
        {
            IList<IList<ChartView>> monthlyChart = await BlogService.GetChart((int)Type, (int)MyChartType.Monthly, OffSet, Username);
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var preresult =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day, s.Date.Hour, 0, 0)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Total = g.Average(x => x.Total),
                    };

                var result =
                    from s in preresult
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }

                preresult =
                    from s in monthlyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day, s.Date.Hour, 0, 0)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Total = g.Average(x => x.Total),
                    };

                result =
                    from s in preresult
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }

                result =
                    from s in monthlyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyPrevData.Add(item.Value.Value);
                }
            }
            LoadCompleteM = true;
            StateHasChanged();
        }

        protected async Task Load()
        {
            Title = GetEnumDescription(Type);

            await LoadHourly(Username);

            await LoadDaily(Username);

            await LoadMonthly(Username);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
