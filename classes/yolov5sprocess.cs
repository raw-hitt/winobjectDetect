using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolov5Net.Scorer;

namespace winobjectDetect.classes
{
    public class yolov5sprocess
    {

        private Yolov5 yolo;

        public yolov5sprocess()
        {
            yolo = new Yolov5(AppDomain.CurrentDomain.BaseDirectory + "best.onnx");
            yolo.SetupLabels(new string[] { "car", "person" });

        }


        internal Tuple<Image, List<YoloPrediction>> ProcessImages(System.Drawing.Image image)
        {

            List<YoloPrediction> predictions = yolo.Predict(image);


            var graphics = Graphics.FromImage(image);

            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);

                graphics.DrawRectangles(new Pen(prediction.Label.Color, 1),
                    new[] { prediction.Rectangle });

                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

                graphics.DrawString($"{prediction.Label.Name} ({score})",
                    new Font("Consolas", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
                    new PointF(x, y));
            }

            return Tuple.Create(image, predictions);
        }

        internal void Dispose()
        {
            yolo.Dispose();
        }
    }
}
