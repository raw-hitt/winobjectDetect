using System.Collections.Generic;

namespace Yolov5Net.Scorer.Models.Abstract
{
    /// <summary>
    /// Model descriptor.
    /// </summary>
    public class YoloModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        public int Dimensions { get; set; }

        public int[] Strides { get; set; }
        public int[][][] Anchors { get; set; }
        public int[] Shapes { get; set; }

        public float Confidence { get; set; }
        public float MulConfidence { get; set; }
        public float Overlap { get; set; }

        public string[] Outputs { get; set; }
        public List<YoloLabel> Labels { get; set; }
        public bool UseDetect { get; set; }
    }
}
