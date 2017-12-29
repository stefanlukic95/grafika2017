using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using AssimpSample;
using Assimp;
using System.Globalization;

namespace RacunarskaGrafikaP
{

    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;


        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {

            InitializeComponent();


            try
            {

                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "C:\\Users\\Stefan\\Documents\\Visual Studio 2017\\Projects\\RacunarskaGrafikaP\\RacunarskaGrafikaP\\Model\\"), "castle2.obj", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "C:\\Users\\Stefan\\Documents\\Visual Studio 2017\\Projects\\RacunarskaGrafikaP\\RacunarskaGrafikaP\\Model\\"), "Arrow shot.fbx", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.K:
                    if (m_world.RotationX >= -20.0f)
                    {
                        m_world.RotationX -= 5.0f;
                    }
                    break;
                case Key.I:
                    if (m_world.RotationX <= 65.0f)
                    {
                        m_world.RotationX += 5.0f;
                    }
                    break;
                case Key.J: m_world.RotationY -= 5.0f; break;
                case Key.L: m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance -= 700.0f; break;
                case Key.Subtract: m_world.SceneDistance += 700.0f; break;

                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;
            }
        }
        private void targetValueChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float val = float.Parse(targetTranslateVal.Text, CultureInfo.InvariantCulture.NumberFormat);

                if (m_world != null)
                {
                    m_world.TargetValueTranslate = val;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Unesite broj:\n" + ex.Message, "GRESKA", MessageBoxButton.OK);
            }
        }

        private void wallValueChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float val = float.Parse(wallVal.Text, CultureInfo.InvariantCulture.NumberFormat);

                if (m_world != null)
                {
                    m_world.wallValue = val;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Unesite broj:\n" + ex.Message, "GRESKA", MessageBoxButton.OK);
            }
        }

        private void arrowValueChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float val = float.Parse(arrowVal.Text, CultureInfo.InvariantCulture.NumberFormat);

                if (m_world != null)
                {
                    m_world.arrowValue = val;
                }

            }
            catch (Exception ex)
            {
                // MessageBox.Show("Unesite broj:\n" + ex.Message, "GRESKA", MessageBoxButton.OK);
            }
        }
        
    }
}
