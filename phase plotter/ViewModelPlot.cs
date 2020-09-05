using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PhasePlotter.Graphics;
using System;
using System.Collections.Generic;

namespace PhasePlotter
{
    public class ViewModelPlot : BaseViewModel
    {
        private int _arrows;
        private double _arrowSize;
        private Point _bottomright;
        private int _chunks;
        private double _gridSpan;
        private bool _inEdit;
        private PlotModel _plotModel;
        private Point _topleft;
        private Func<double, double, double> _xt;
        private Func<double, double, double> _yt;
        private LinearAxis axisX;
        private LinearAxis axisY;

        public ViewModelPlot(Func<double, double, double> xt, Func<double, double, double> yt, double gridSpan, int steps, int chunks, int arrows, double arrowSize, bool fromcenter)
        {
            PlotModel = new PlotModel
            {
                PlotAreaBorderColor = OxyColor.FromRgb(230, 230, 230),
            };
            _xt = xt;
            _yt = yt;
            _arrowSize = arrowSize;
            _chunks = chunks;
            _arrows = arrows;
            _gridSpan = gridSpan;

            axisY = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 0.5,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(240, 240, 240),
                MinimumPadding = 0.05,
                MaximumPadding = 0.05,
                Position = AxisPosition.Left,
                Title = "y",
            };
            axisX = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 0.5,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(240, 240, 240),
                MinimumPadding = 0.05,
                MaximumPadding = 0.05,
                Position = AxisPosition.Bottom,
                Title = "x"
            };

            PlotModel.Axes.Add(axisX);
            PlotModel.Axes.Add(axisY);

            PlotModel.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    var y = axisY.InverseTransform(e.Position.Y);
                    var x = axisX.InverseTransform(e.Position.X);

                    DrawLines(x, x, y, y);

                    PlotModel.RefreshPlot(false);
                    e.Handled = true;
                }
            };

            MakePlot(steps, fromcenter);
        }

        public bool InEditMode
        {
            get
            {
                return _inEdit;
            }
            set
            {
                if (_inEdit != value)
                {
                    _inEdit = value;
                    foreach (LineSeries serie in PlotModel.Series)
                    {
                        serie.StrokeThickness = Thickness;
                    }

                    PlotModel.RefreshPlot(false);
                    OnPropertyChanged(() => InEditMode);
                }
            }
        }

        public PlotModel PlotModel
        {
            get
            {
                return _plotModel;
            }

            set
            {
                _plotModel = value;
                OnPropertyChanged(() => PlotModel);
            }
        }

        private double Thickness
        {
            get { return InEditMode ? 5 : 1; }
        }

        private IEnumerable<DataPoint> DrawArrow(Vector direction)
        {
            var result = new List<DataPoint>();
            var v = direction.Normalize() * _arrowSize;
            var r = v.RotateDeg(135);
            var l = v.RotateDeg(-135);

            result.Add(new DataPoint(r.Finish.X, r.Finish.Y));
            result.Add(new DataPoint(r.Start.X, r.Start.Y));
            result.Add(new DataPoint(l.Finish.X, l.Finish.Y));
            result.Add(new DataPoint(l.Start.X, l.Start.Y));

            return result;
        }

        private void DrawLines(double xFrom, double xTo, double yFrom, double yTo)
        {
            for (double x = xFrom; x <= xTo; x += _gridSpan)
                for (double y = yFrom; y <= yTo; y += _gridSpan)
                {
                    var series = new LineSeries(OxyColors.DarkBlue);
                    series.CanTrackerInterpolatePoints = false;
                    series.StrokeThickness = Thickness;
                    Subscribe(series);
                    foreach (var point in GetPointsOfLineFrom(x, y))
                    {
                        series.Points.Add(point);
                    }
                    PlotModel.Series.Add(series);
                }
        }

        private IEnumerable<DataPoint> GetPointsOfLineFrom(double x, double y)
        {
            var result = new List<DataPoint>();
            result.Add(new DataPoint(x, y));
            Point point = null;
            Vector v = null;
            int arrowStep;
            if (_arrows > 0 && _chunks / _arrows > 0)
                arrowStep = _chunks / _arrows;
            else
                arrowStep = _chunks;

            for (int i = 0; i < _chunks && _topleft.X <= x && _bottomright.X >= x && _topleft.Y >= y && _bottomright.Y <= y; i++)
            {
                point = new Point(_xt(x, y), _yt(x, y));
                v = new Vector(new Point(x, y), point);
                v = v.Normalize();

                result.Add(new DataPoint(v.Finish.X, v.Finish.Y));
                x = v.Finish.X;
                y = v.Finish.Y;

                if (i > 0 && i % arrowStep == 0)
                {
                    result.AddRange(DrawArrow(new Vector(new Point(x, y), new Point(_xt(x, y), _yt(x, y)))));
                }
            }

            if (_arrows > 0)
                result.AddRange(DrawArrow(new Vector(new Point(x, y), new Point(_xt(x, y), _yt(x, y)))));
            return result;
        }

        private void MakePlot(int steps, bool fromcenter)
        {
            var fromto = _gridSpan * steps;
            double range;
            if (fromcenter)
            {
                range = fromto * 2;
            }
            else
            {
                range = fromto + fromto * 0.02;
            }

            _topleft = new Point(-range, range);
            _bottomright = new Point(range, -range);

            if (fromcenter)
            {
                DrawLines(-fromto, fromto, -fromto, fromto);
            }
            else
            {
                DrawLines(-fromto, fromto, -fromto, -fromto);
                DrawLines(-fromto, fromto, fromto, fromto);
                DrawLines(-fromto, -fromto, -fromto, fromto);
                DrawLines(fromto, fromto, -fromto, fromto);
            }
        }

        private void Subscribe(LineSeries series)
        {
            EventHandler<OxyMouseEventArgs> MouseDown = (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    series.LineStyle = LineStyle.Dot;

                    PlotModel.RefreshPlot(false);
                    e.Handled = true;
                }
            };

            EventHandler<OxyMouseEventArgs> MouseUp = (s, e) =>
            {
                PlotModel.Series.Remove(series);
                if (InEditMode)
                {
                    var y = axisY.InverseTransform(e.Position.Y);
                    var x = axisX.InverseTransform(e.Position.X);
                    DrawLines(x, x, y, y);
                }

                PlotModel.RefreshPlot(false);
                e.Handled = true;
            };

            series.MouseDown += MouseDown;
            series.MouseUp += MouseUp;
        }
    }
}