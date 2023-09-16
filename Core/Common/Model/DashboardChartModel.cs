using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class DashboardChartModel
    {
        public int? TotalApplicantByJobTitle { get; set; }
        public List<JobApplicationByChannel>? JobApplicationByChannels { get; set; }
        public List<JobApplicationByJobTitleChart>? JobApplicationByJobTitles { get; set; }
        public List<JobApplicationByChannel>? JobApplicationBySkillSet { get; set; }
        public List<JobApplicationByAgeRange>? JobApplicationByAgeRange { get; set; }
        public List<ApplicantTurnOver>? ApplicantTurnOver { get; set; }
        public List<ApplicantOverTime>? ApplicantOverTime { get; set; }
    }

    public class JobApplicationByChannel
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }


    public class JobApplicationBySkillSet
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }


    public class JobApplicationByJobTitleChart
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }

    public class JobApplicationByAgeRange
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }

    public class ApplicantTurnOver
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }
    public class ApplicantOverTime
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }
    public class DashBoardListGraphData
    {
        public string Label { get; set; }
        public int Data { get; set; }
    }

    public class EmployeeTurnOverDashBoardListGraphData
    {
        public List<DashBoardListGraphData> CurrentYearData { get; set; }
        public List<DashBoardListGraphData> PreviousYearData { get; set; }
    }

    public class DashBoardListGraphDataForDate
    {
        public DateTime Label { get; set; }
        public int Data { get; set; }
    }

    public class DashBoardListGraphDataForDateYear
    {
        public int? Label { get; set; }
        public int Data { get; set; }
    }

    public class DashBoardFilter
    {
        public DateTime? DateFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Channel { get; set; }
        public int? Year { get; set; }

    }



    public class RoleSkill
    {
        public string SkillName { get; set; }
        public double PercentageShare { get; set; }
        public int SkillCount { get; set; }

    }

    public class SkillWithPercentage
    {
        public string Name { get; set; }
        public double Percentage { get; set; }
    }

}
