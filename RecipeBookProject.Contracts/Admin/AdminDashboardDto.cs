using System;
using System.Collections.Generic;

namespace RecipeBookProject.Contracts.Admin;

public class AdminDashboardDto
{
    public int TotalProducts { get; set; }
    public int PendingCount { get; set; }
    public int TotalComments { get; set; }
    public int TotalReports { get; set; }

    public List<WeeklyPointDto> WeeklyPublished { get; set; } = new();
    public List<CategoryShareDto> CategoryDistribution { get; set; } = new();
    public List<ReportedItemDto> ReportedTop { get; set; } = new();
    public List<CommentItemDto> RecentComments { get; set; } = new();
}

public class WeeklyPointDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class CategoryShareDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percent { get; set; }
}

public class ReportedItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CommentItemDto
{
    public string Author { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Avatar { get; set; }
}



