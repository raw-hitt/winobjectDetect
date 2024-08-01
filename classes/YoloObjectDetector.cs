using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace ObjectDetection
{
    public class YoloPrediction2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Confidence { get; set; }
        public int ClassIndex { get; set; }
    }

    public class YoloObjectDetector
    {
        private readonly InferenceSession _session;

        public YoloObjectDetector()
        {
            string modelPath = AppDomain.CurrentDomain.BaseDirectory + "yolov5c_old.onnx";
            _session = new InferenceSession(modelPath);
        }

        public (Bitmap, List<YoloPrediction2>) DetectObjects(Bitmap image)
        {
            // Preprocess the image
            var inputData = PreprocessImage(image);

            // Create a tensor from the input data
            var inputTensor = new DenseTensor<float>(inputData, new[] { 1, 3, 640, 640 });

            // Create an inference input
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("images", inputTensor)
            };

            // Run inference
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs);

            // Get output and process results
            var output = results.First().AsEnumerable<float>().ToArray();
            var predictions = ProcessOutput(output);

            // Annotate image
            var annotatedImage = AnnotateImage(image, predictions);

            return (annotatedImage, predictions);
        }

        private float[] PreprocessImage(Bitmap image)
        {
            // Resize image to 640x640
            Bitmap resized = new Bitmap(image, new Size(640, 640));

            // Normalize pixel values to [0, 1]
            float[] inputData = new float[3 * 640 * 640];
            int idx = 0;

            for (int y = 0; y < resized.Height; y++)
            {
                for (int x = 0; x < resized.Width; x++)
                {
                    Color pixel = resized.GetPixel(x, y);
                    inputData[idx++] = pixel.R / 255.0f;
                    inputData[idx++] = pixel.G / 255.0f;
                    inputData[idx++] = pixel.B / 255.0f;
                }
            }

            return inputData;
        }

        private List<YoloPrediction2> ProcessOutput(float[] output)
        {
            var predictions = new List<YoloPrediction2>();
            int numDetections = output.Length / 85; // Each detection has 85 values (x, y, width, height, confidence, 80 class probabilities)

            for (int i = 0; i < numDetections; i++)
            {
                float confidence = output[i * 85 + 4];
                if (confidence > 0.5) // Threshold for filtering weak detections
                {
                    float x = output[i * 85 + 0];
                    float y = output[i * 85 + 1];
                    float width = output[i * 85 + 2];
                    float height = output[i * 85 + 3];

                    // Find the class with the highest probability
                    int classIndex = 0;
                    float maxProb = 0.0f;
                    for (int j = 5; j < 85; j++)
                    {
                        if (output[i * 85 + j] > maxProb)
                        {
                            maxProb = output[i * 85 + j];
                            classIndex = j - 5;
                        }
                    }

                    predictions.Add(new YoloPrediction2
                    {
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                        Confidence = confidence,
                        ClassIndex = classIndex
                    });
                }
            }

            return predictions;
        }

        private Bitmap AnnotateImage(Bitmap image, List<YoloPrediction2> predictions)
        {
            Graphics graphics = Graphics.FromImage(image);
            foreach (var prediction in predictions)
            {
                // Calculate bounding box coordinates
                float x1 = prediction.X - (prediction.Width / 2);
                float y1 = prediction.Y - (prediction.Height / 2);
                float x2 = prediction.X + (prediction.Width / 2);
                float y2 = prediction.Y + (prediction.Height / 2);

                // Draw bounding box
                RectangleF rect = new RectangleF(x1, y1, x2 - x1, y2 - y1);
                graphics.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);

                // Draw confidence score
                string label = $"Class: {prediction.ClassIndex}, Score: {prediction.Confidence:F2}";
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Red, new PointF(x1, y1));
            }

            return image;
        }
    }
}
