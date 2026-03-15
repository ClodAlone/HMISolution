using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShaderEffects
{
    public class ReflectionEffect : ShaderEffect
    {
        
        #region Constructors

        public ReflectionEffect()
        {
            this.PixelShader = new PixelShader();
            this.PixelShader.UriSource = Global.MakePackUri("ReflectionEffect.ps");

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(CenterXProperty);
            UpdateShaderValue(LeftAngleProperty);
            UpdateShaderValue(RightAngleProperty);
            UpdateShaderValue(DeepProperty);
        }

        #endregion Constructors

#if SILVERLIGHT

        public void UpdateBinding(FrameworkElement uie)
        {
            if (uie == null)
                return;

            var binding = new System.Windows.Data.Binding()
            {
                Source = uie,
                Path = new PropertyPath("ActualHeight")
            };

            System.Windows.Data.BindingOperations.SetBinding(this, InputHeightProperty, binding);
            PaddingBottom = uie.Height;
        }

#endif

        #region Dependency Properties

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ReflectionEffect), 0);

        public double InputHeight
        {
            get { return (double)GetValue(InputHeightProperty); }
            set
            {
                SetValue(InputHeightProperty, value);
            }
        }

        public static readonly DependencyProperty InputHeightProperty =
            DependencyProperty.Register("InputHeight", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata(
#else
 new UIPropertyMetadata(
#endif
new PropertyChangedCallback(
                        (DependencyObject o, DependencyPropertyChangedEventArgs e) =>
                        {
                            ((ReflectionEffect)o).PaddingBottom = (double)e.NewValue;
                        }
                    )
                )
            );

        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterX", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata
#else
 new UIPropertyMetadata
#endif
(0.5, PixelShaderConstantCallback(0)));

        public double LeftAngle
        {
            get { return (double)GetValue(LeftAngleProperty); }
            set { SetValue(LeftAngleProperty, value); }
        }

        public static readonly DependencyProperty LeftAngleProperty =
            DependencyProperty.Register("LeftAngle", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata
#else
 new UIPropertyMetadata
#endif
(0.0, PixelShaderConstantCallback(1)));

        public double RightAngle
        {
            get { return (double)GetValue(RightAngleProperty); }
            set { SetValue(RightAngleProperty, value); }
        }

        public static readonly DependencyProperty RightAngleProperty =
            DependencyProperty.Register("RightAngle", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata
#else
 new UIPropertyMetadata
#endif
(0.0, PixelShaderConstantCallback(2)));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata
#else
 new UIPropertyMetadata
#endif
(0.0, PixelShaderConstantCallback(3)));


        public double Deep
        {
            get { return (double)GetValue(DeepProperty); }
            set { SetValue(DeepProperty, value); }
        }

        public static readonly DependencyProperty DeepProperty =
            DependencyProperty.Register("Deep", typeof(double), typeof(ReflectionEffect),
#if SILVERLIGHT
 new PropertyMetadata
#else
 new UIPropertyMetadata
#endif
(0.5, PixelShaderConstantCallback(4)));

        #endregion Dependency Properties
    }
}