﻿using CS6232_G2.Controller;
using CS6232_G2.Model;
using CS6232_G2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace CS6232_G2.View
{
    public partial class RoutineCheckup : Form
    {
        private readonly RoutineCheckController _routineCheckController;
        private PatientVisit visit;
        private TestController _testController;
        private List<Test> _tests;
        private List<Test> _orderedTests;
        private Nurse _nurse;
        private NurseController _nurseController;

        public RoutineCheckup()
        {
            InitializeComponent();
            _routineCheckController = new RoutineCheckController();
            visit = new PatientVisit();
            this._testController = new TestController();
            this._orderedTests = new List<Test>();
            this._nurseController = new NurseController();
            _nurse = _nurseController.GetNurseByLogin(LoginDAL.GetCurrentLogin());
        }



        private decimal GetDecimal2(string number, string source)
        {
            try
            {
                decimal value = Decimal.Parse(number);
                if (value < 0)
                {
                    throw new FormatException();
                }
                return Decimal.Round(value, 2);
            }
            catch (Exception)
            {
                throw new FormatException("Please enter a valid " + source);
            }
        }

        private decimal GetDecimal1(string number, string source)
        {
            try
            {
                decimal value = Decimal.Parse(number);
                if (value < 0)
                {
                    throw new FormatException();
                }
                return Decimal.Round(value, 1);
            }
            catch (Exception)
            {
                throw new FormatException("Please enter a valid " + source);
            }
        }
        private int GetInt(string number, string source)
        {
            try
            {
                int id = -1;
                id = Int32.Parse(number);
                if (id < 0)
                {
                    throw new FormatException();
                }
                return id;
            }
            catch (Exception)
            {
                throw new FormatException("Please enter a valid " + source);
            }
        }
        private PatientVisit PatientVisit()
        {
            PatientVisit newVisit = new PatientVisit
            {
                Height = GetDecimal2(heightTextBox.Text, "height"),
                Weight = GetDecimal2(weightTextBox.Text, "weight"),
                Systolic = GetInt(sysTextBox.Text, "systolic number"),
                Diastolic = GetInt(diaTextBox.Text, "diastolic number"),
                Temperature = GetDecimal1(tempTextBox.Text, "temperature"),
                Pulse = GetInt(pulseTextBox.Text, "pulse")
            };

            if (symptomsTextBox.Text.Length > 254)
            {
                DialogResult dialogResult = MessageBox.Show("only 150 letters are allowed for symptoms. Would you like to trim to 150?",
                    "The symptoms description is too big!", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    newVisit.Symptoms = symptomsTextBox.Text.Substring(0, 149);
                    symptomsTextBox.Text = symptomsTextBox.Text.Substring(0, 149);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (String.IsNullOrEmpty(symptomsTextBox.Text))
            {
                throw new Exception("Please fill out symptoms");
            }
            else
            {
                newVisit.Symptoms = symptomsTextBox.Text;
            }

            if (iDiagnosisTextBox.Text.Length > 45)
            {
                DialogResult dialogResult = MessageBox.Show("only 45 letters are allowed for initial diagnosis. Would you like to trim to 45?",
                    "The description is too long!", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    newVisit.InitialDiagnosis = iDiagnosisTextBox.Text.Substring(0, 44);
                    iDiagnosisTextBox.Text = iDiagnosisTextBox.Text.Substring(0, 44);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (String.IsNullOrEmpty(iDiagnosisTextBox.Text))
            {
                throw new Exception("Please fill out an initial diagnosis");
            }
            else
            {
                newVisit.InitialDiagnosis = iDiagnosisTextBox.Text;
            }

            if (fDiagnosesTextBox.Text.Length > 45)
            {
                DialogResult dialogResult = MessageBox.Show("only 44 letters are allowed for final diagnosis. Would you like to trim to 45?",
                    "The description is too long!", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    newVisit.FinalDiagnosis = fDiagnosesTextBox.Text.Substring(0, 44);
                    fDiagnosesTextBox.Text = fDiagnosesTextBox.Text.Substring(0, 44);
                }
                else
                {
                    throw new Exception();
                }
            }
            else if (String.IsNullOrEmpty(fDiagnosesTextBox.Text))
            {
                newVisit.FinalDiagnosis = "Empty!";
            }
            else
            {
                newVisit.FinalDiagnosis = fDiagnosesTextBox.Text;
            }

            return newVisit;
        }

        private void saveVisitButton_Click(object sender, EventArgs e)
        {
            try
            {
                PatientVisit firstVisit = PatientVisit();

                if (_routineCheckController.RoutineVisit(firstVisit))
                {
                    errorLabel.Text = "The checkup has been successfully entered";
                }
                else
                {
                    errorLabel.Text = "The checkup wasn't entered. There was an error.";
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }
        private void HandleDecimalInput(System.Windows.Forms.TextBox textBox, KeyPressEventArgs e, int maxIntegerDigits, int maxDecimalDigits)
        {
            // Check if the key is a valid numeric key (0-9), decimal point symbol ('.'), or Backspace key
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                MessageBox.Show("Please enter only numbers.");
            }
            else
            {
                string text = textBox.Text;

                if (e.KeyChar == '.' && text.Contains('.'))
                {
                    e.Handled = true;
                    MessageBox.Show("Please enter only one decimal point.");
                }
                else if (text.IndexOf('.') != -1 && text.Substring(text.IndexOf('.')).Length > maxDecimalDigits && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                    MessageBox.Show($"Please enter a maximum of {maxDecimalDigits} decimal places.");
                }
                else if (text.IndexOf('.') == -1 && text.Length >= maxIntegerDigits && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                    MessageBox.Show($"Please enter a maximum of {maxIntegerDigits} digits before the decimal point.");
                }
                else if (text.IndexOf('.') != -1 && text.IndexOf('.') > maxIntegerDigits && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                    MessageBox.Show($"Please enter a maximum of {maxIntegerDigits} digits before the decimal point.");
                }
            }
        }

        private void heightTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleDecimalInput(heightTextBox, e, 3, 2);
        }

        private void weightTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleDecimalInput(weightTextBox, e, 3, 2);
        }

        private void sysTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
                MessageBox.Show("Please enter only numbers.");
            }
        }

        private void diaTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
                MessageBox.Show("Please enter only numbers.");
            }
        }

        private void tempTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleDecimalInput(tempTextBox, e, 3, 1);
        }
        private void HandleTextInput(System.Windows.Forms.TextBox textBox, KeyPressEventArgs e, int maxChars)
        {
            if (!Char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '/' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
                MessageBox.Show("Only letters, digits, '-', and '/' are allowed.");
            }
            else if (textBox.Text.Length >= maxChars && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
                MessageBox.Show($"Please enter no more than {maxChars} characters.");
            }
        }

        private void pulseTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTextInput(pulseTextBox, e, 5);
        }

        private void RoutineCheckup_Load(object sender, EventArgs e)
        {
            this._tests = this._testController.GetAllTests();
            this.selectLabTestComboBox.DataSource = this._tests;
            this.selectLabTestComboBox.DisplayMember = "TestName";
            this.selectLabTestComboBox.SelectedIndex = 0;
            this.testBindingSource.DataSource = this._orderedTests;
        }

        private void addTestButton_Click(object sender, EventArgs e)
        {
            this.testBindingSource.Add(this._tests[this.selectLabTestComboBox.SelectedIndex]);
        }

        private void removeTestButton_Click(object sender, EventArgs e)
        {
            this.testBindingSource.RemoveAt(this.testDataGridView.SelectedRows.Count - 1);
        }
    }
}

