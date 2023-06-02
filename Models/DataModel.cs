using System;
using System.Collections.Generic;

namespace KR_HypothesisCheck.Models;

public class DataModel
{
    public List<double>? StatisticData { get; set; }
    public List<double>? LabelData { get; set; }
    public List<double>? Distribution { get; set; }
    public double moda { get; set; }
    public double median { get; set; }
    public double AvgSelect { get; set; }
    public bool Conclusion { get; set; }
}