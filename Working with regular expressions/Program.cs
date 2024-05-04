using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class Location
{
    public string Name { get; set; }
    public float Coefficient { get; set; }
    public string Type { get; set; }
    public List<Location> Route { get; set; }

    public Location()
    {
        Route = new List<Location>();
    }

    public bool ValidateData()
    {
        Regex nameRegex = new Regex(@"^[a-zA-Zа-яА-Я\- ]+$");
        bool nameValid = nameRegex.IsMatch(Name);
        bool coeffValid = Coefficient >= 0.01f && Coefficient <= 5.0f;
        bool typeValid = !string.IsNullOrEmpty(Type);
        return nameValid && coeffValid && typeValid;
    }
}

public class LocationEntryForm : Form
{
    private TextBox nameTextBox;
    private TextBox coeffTextBox;
    private ComboBox typeComboBox;
    private Button saveButton;
    public Location LocationData { get; private set; }

    public LocationEntryForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Label nameLabel = new Label { Text = "Имя местности:", Left = 20, Top = 20, Width = 100 };
        nameTextBox = new TextBox { Left = 130, Top = 20, Width = 150 };

        Label coeffLabel = new Label { Text = "Коэффициент:", Left = 20, Top = 50, Width = 100 };
        coeffTextBox = new TextBox { Left = 130, Top = 50, Width = 150 };

        Label typeLabel = new Label { Text = "Тип местности:", Left = 20, Top = 80, Width = 100 };
        typeComboBox = new ComboBox { Left = 130, Top = 80, Width = 150 };
        typeComboBox.Items.AddRange(new string[] { "Лес", "Поле", "Город", "Пустыня" });

        saveButton = new Button { Text = "Сохранить", Left = 20, Top = 110, Width = 100 };
        saveButton.Click += SaveButton_Click;

        Controls.AddRange(new Control[] { nameLabel, nameTextBox, coeffLabel, coeffTextBox, typeLabel, typeComboBox, saveButton });

        Text = "Добавить новое местоположение";
        Width = 300;
        Height = 200;
        AutoSize = true;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        // Проверка наличия имени местности
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Пожалуйста, введите имя местности.");
            return;
        }

        // Проверка выбора типа местности
        if (typeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Пожалуйста, выберите тип местности.");
            return;
        }

        if (!float.TryParse(coeffTextBox.Text, out float coeff) || coeff < 0.01f || coeff > 5.0f)
        {
            MessageBox.Show("Коэффициент должен быть числом от 0.01 до 5.0");
            return;
        }

        LocationData = new Location
        {
            Name = nameTextBox.Text,
            Coefficient = coeff,
            Type = typeComboBox.SelectedItem.ToString()
        };

        if (!LocationData.ValidateData())
        {
            MessageBox.Show("Неверные данные. Пожалуйста, проверьте введенные значения.");
        }
        else
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }


    public class MainForm : Form
    {
        private Button addLocationButton;
        private Button calculateButton;
        private Button resetButton;
        private TextBox baseSpeedTextBox;
        private Label baseSpeedLabel;
        private List<Location> locations = new List<Location>();

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            addLocationButton = new Button { Text = "Добавить местоположение", Left = 20, Top = 20, Width = 230 };
            baseSpeedLabel = new Label { Text = "Базовая скорость (км/ч):", Left = 20, Top = 60, Width = 180 };
            baseSpeedTextBox = new TextBox { Left = 210, Top = 55, Width = 40 };

            calculateButton = new Button { Text = "Рассчитать маршрут", Left = 20, Top = 100, Width = 230 };
            resetButton = new Button { Text = "Сброс", Left = 20, Top = 140, Width = 230 };

            addLocationButton.Click += AddLocationButton_Click;
            calculateButton.Click += CalculateButton_Click;
            resetButton.Click += ResetButton_Click;

            Controls.AddRange(new Control[] { addLocationButton, baseSpeedLabel, baseSpeedTextBox, calculateButton, resetButton });

            Text = "Ввод данных о местоположениях";
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void AddLocationButton_Click(object sender, EventArgs e)
        {
            var locationForm = new LocationEntryForm();
            if (locationForm.ShowDialog() == DialogResult.OK)
            {
                locations.Add(locationForm.LocationData);
                MessageBox.Show("Местоположение добавлено успешно.");
            }
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (locations.Count == 0)
            {
                MessageBox.Show("Сначала добавьте местоположения.");
                return;
            }

            if (!float.TryParse(baseSpeedTextBox.Text, out float baseSpeed) || baseSpeed <= 0)
            {
                MessageBox.Show("Введите корректную базовую скорость (положительное число).");
                return;
            }

            float totalTime = 0f;
            foreach (var location in locations)
            {
                totalTime += 1.0f / (baseSpeed * location.Coefficient);
            }

            int hours = (int)totalTime;
            int minutes = (int)((totalTime - hours) * 60);
            int seconds = (int)(((totalTime - hours) * 60 - minutes) * 60);

            MessageBox.Show($"Общее время пути: {hours} часов {minutes} минут {seconds} секунд");
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            locations.Clear();
            baseSpeedTextBox.Text = "";
            MessageBox.Show("Данные сброшены.");
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
