using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Numerics;

namespace PhasePlotter
{
    public class MainViewModel : BaseViewModel
    {
        private double _a11;
        private double _a12;
        private double _a21;
        private double _a22;

        private Complex _root1;
        private Complex _root2;

        private int _arrows;
        private double _arrSize;
        private int _chunks;
        private int _steps;
        private double _gridStep;

        private ViewModelPlot _plot;

        private string _edit;

        public MainViewModel()
        {
            A11 = -7.5;
            A12 = 11;
            A21 = -9.7;
            A22 = 5;

            GridStep = 30;
            Chunks = 100;
            Arrows = 1;
            ArrowSize = 2.5;
            Steps = 2;

            SetEditLabel();
        }
        public double A11
        {
            get
            {
                return _a11;
            }
            set
            {
                if (_a11 != value)
                {
                    _a11 = value;
                    CalcLambdas();
                    OnPropertyChanged(() => A11);
                }
            }
        }

        public double A12
        {
            get
            {
                return _a12;
            }
            set
            {
                if (_a12 != value)
                {
                    _a12 = value;
                    CalcLambdas();
                    OnPropertyChanged(() => A12);
                }
            }
        }

        public double A21
        {
            get
            {
                return _a21;
            }
            set
            {
                if (_a21 != value)
                {
                    _a21 = value;
                    CalcLambdas();
                    OnPropertyChanged(() => A21);
                }
            }
        }

        public double A22
        {
            get
            {
                return _a22;
            }
            set
            {
                if (_a22 != value)
                {
                    _a22 = value;
                    CalcLambdas();
                    OnPropertyChanged(() => A22);
                }
            }
        }

        public ViewModelPlot Plot
        {
            get
            {
                return _plot;
            }
            set
            {
                if (_plot != value)
                {
                    _plot = value;
                    OnPropertyChanged(() => Plot);
                }
            }
        }

        public int Arrows
        {
            get
            {
                return _arrows;
            }
            set
            {
                if (_arrows != value)
                {
                    if (value < 0) value = 0;
                    _arrows = value;
                    OnPropertyChanged(() => Arrows);
                }
            }
        }

        public double ArrowSize
        {
            get
            {
                return _arrSize;
            }
            set
            {
                if (_arrSize != value)
                {
                    if (value < 0.005) value = 0.005;
                    _arrSize = value;
                    OnPropertyChanged(() => ArrowSize);
                }
            }
        }
        /// <summary>
        /// Длина линии
        /// </summary>
        public int Chunks
        {
            get
            {
                return _chunks;
            }
            set
            {
                if (_chunks != value)
                {
                    if (value < 1) value = 1;

                    _chunks = value;
                    OnPropertyChanged(() => Chunks);
                }
            }
        }
        /// <summary>
        /// Шаг сетки
        /// </summary>
        public double GridStep
        {
            get
            {
                return _gridStep;
            }
            set
            {
                if (_gridStep != value)
                {
                    if (value < 0.001) value = 0.001;
                    _gridStep = value;
                    OnPropertyChanged(() => GridStep);
                }
            }
        }

        public int Steps
        {
            get
            {
                return _steps;
            }
            set
            {
                if (_steps != value)
                {
                    if (value < 1) value = 1;

                    _steps = value;
                    OnPropertyChanged(() => Steps);
                }
            }
        }
        public Complex Root1
        {
            get
            {
                return _root1;
            }
            set
            {
                if (_root1 != value)
                {
                    _root1 = value;
                    OnPropertyChanged(() => Root1);
                }
            }
        }

        public Complex Root2
        {
            get
            {
                return _root2;
            }
            set
            {
                if (_root2 != value)
                {
                    _root2 = value;
                    OnPropertyChanged(() => Root2);
                }
            }
        }
        public RelayCommand DrawFromCenterCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Draw(true);
                });
            }
        }
        public RelayCommand DrawFromEdgesCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Draw(false);

                });
            }
        }
        public RelayCommand ToggleEditCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Plot.InEditMode = !Plot.InEditMode;
                    SetEditLabel();
                }, () => Plot != null);
            }
        }

        public string EditLabel
        {
            get
            {
                return _edit;
            }
            set
            {
                if (_edit != value)
                {
                    _edit = value;
                    OnPropertyChanged(() => EditLabel);
                }
            }
        }

        private void Draw(bool fromCenter)
        {
            var m = CreateMatrix();

            Func<double, double, double> xt = (x, y) => { return m[0, 0] * x + m[0, 1] * y; };
            Func<double, double, double> yt = (x, y) => { return m[1, 0] * x + m[1, 1] * y; };

            Plot = new ViewModelPlot(xt, yt, GridStep, Steps, Chunks, Arrows, ArrowSize, fromCenter);
        }

        private void CalcLambdas()
        {
            var m = CreateMatrix();
            var eval = m.Evd().EigenValues();
            var l1 = eval[0];
            var l2 = eval[1];

            Root1 = l1;
            Root2 = l2;
        }

        private void SetEditLabel()
        {
            if (Plot != null && Plot.InEditMode)
            {
                EditLabel = "Закончить перемещать линии";
            }
            else
            {
                EditLabel = "Начать перемещать линии";
            }
        }
        private DenseMatrix CreateMatrix()
        {
            var m = DenseMatrix.Create(2, 2, (a, b) => 0);
            m[0, 0] = A11;
            m[0, 1] = A12;
            m[1, 0] = A21;
            m[1, 1] = A22;
            return m;
        }
    }
}