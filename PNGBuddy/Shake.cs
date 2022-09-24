using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PNGBuddy
{
    public class ShakeBehavior : Behavior<Image>
    {
        private const double DefaultRepeatInterval = 10.0;
        private const double DefaultSpeedRatio = 1.0;

        private const string RepeatIntervalName = "RepeatInterval";
        private const string SpeedRatioName = "SpeedRatio";

        public static readonly DependencyProperty RepeatIntervalProperty =
            DependencyProperty.Register(RepeatIntervalName,
                                        typeof(double),
                                        typeof(ShakeBehavior),
                                        new PropertyMetadata(DefaultRepeatInterval));

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register(SpeedRatioName,
                                        typeof(double),
                                        typeof(ShakeBehavior),
                                        new PropertyMetadata(DefaultSpeedRatio));

        /// <summary>
        /// Gets or sets the time interval in in seconds between each shake.
        /// </summary>
        /// <value>
        /// The time interval in in seconds between each shake.
        /// </value>
        /// <remarks>
        /// If interval is less than total shake time, then it will shake
        /// constantly without pause. If this is your intention, simply set
        /// interval to 0.
        /// </remarks>
        public double RepeatInterval
        {
            get { return (double)GetValue(RepeatIntervalProperty); }
            set { SetValue(RepeatIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ratio at which time progresses on the Shakes
        /// Timeline, relative to its parent. 
        /// </summary>
        /// <value> 
        /// The ratio at which time progresses on the Shakes Timeline, relative 
        /// to its parent.
        /// </value>
        /// <remarks> 
        /// If Acceleration or Deceleration are specified, this ratio is the
        /// average ratio over the natural length of the Shake's Timeline. This 
        /// property has a default value of 1.0. If set to zero or less it
        /// will be reset back to th default value.
        /// </remarks>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        private Style? _orignalStyle;
        protected override void OnAttached()
        {
            _orignalStyle = AssociatedObject.Style;
            AssociatedObject.Style = CreateShakeStyle();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Style = _orignalStyle;
        }

        private Style CreateShakeStyle()
        {
            Style newStyle = new Style(AssociatedObject.GetType(), AssociatedObject.Style);
            /**
             * The following will replace/override any existing RenderTransform
             * and RenderTransformOrigin properties on the FrameworkElement
             * once the the new Style is applied to it.
             */
            newStyle.Setters.Add(new Setter(UIElement.RenderTransformProperty, new RotateTransform(0)));
            newStyle.Setters.Add(new Setter(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5)));

            newStyle.Triggers.Add(CreateTrigger());

            return newStyle;
        }

        private DataTrigger CreateTrigger()
        {
            DataTrigger trigger = new DataTrigger
            {
                Binding = new Binding
                {
                    RelativeSource = new RelativeSource
                    {
                        Mode = RelativeSourceMode.FindAncestor,
                        AncestorType = typeof(UIElement)
                    },
                    Path = new PropertyPath(UIElement.IsVisibleProperty)
                },
                Value = true,
            };

            trigger.EnterActions.Add(new BeginStoryboard { Storyboard = CreateStoryboard() });

            return trigger;
        }

        private Storyboard CreateStoryboard()
        {
            double speedRatio = SpeedRatio;

            // Must be greater than zero
            if (speedRatio <= 0.0)
                SpeedRatio = DefaultSpeedRatio;

            Storyboard storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever,
                SpeedRatio = speedRatio
            };

            storyboard.Children.Add(CreateAnimationTimeline());

            return storyboard;
        }

        private Timeline CreateAnimationTimeline()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0).(1)", UIElement.RenderTransformProperty, RotateTransform.AngleProperty));

            int keyFrameCount = 8;
            double timeOffsetInSeconds = 0.25;
            double totalAnimationLength = keyFrameCount * timeOffsetInSeconds;
            double repeatInterval = RepeatInterval;

            // Can't be less than zero and pointless to be less than total length
            if (repeatInterval < totalAnimationLength)
                repeatInterval = totalAnimationLength;

            animation.Duration = new Duration(TimeSpan.FromSeconds(repeatInterval));

            int targetValue = 12;
            for (int i = 0; i < keyFrameCount; i++)
                animation.KeyFrames.Add(new LinearDoubleKeyFrame(i % 2 == 0 ? targetValue : -targetValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * timeOffsetInSeconds))));

            animation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(totalAnimationLength))));
            return animation;
        }
    }
}