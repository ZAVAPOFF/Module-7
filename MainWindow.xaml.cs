using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LearnApp
{
    public partial class MainWindow : Window
    {
        private string connectionString = "server=localhost;user id=root;password=12345678;database=LearnDB;";
        private int selectedWordId = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            List<EnglishWord> words = new List<EnglishWord>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM EnglishWords";
                MySqlCommand command = new MySqlCommand(query, connection);

                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    words.Add(new EnglishWord
                    {
                        Id = reader.GetInt32("id"),
                        Word = reader.GetString("word"),
                        Translation = reader.GetString("translation"),
                        Pronunciation = reader.IsDBNull(reader.GetOrdinal("pronunciation")) ? "" : reader.GetString("pronunciation")
                    });
                }
            }

            WordsDataGrid.ItemsSource = words;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO EnglishWords (word, translation, pronunciation) VALUES (@word, @translation, @pronunciation)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@word", WordTextBox.Text);
                command.Parameters.AddWithValue("@translation", TranslationTextBox.Text);
                command.Parameters.AddWithValue("@pronunciation", PronunciationTextBox.Text);

                connection.Open();
                command.ExecuteNonQuery();
            }
            LoadData();
            ClearFields();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedWordId == -1)
            {
                MessageBox.Show("Please select a word to edit.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE EnglishWords SET word=@word, translation=@translation, pronunciation=@pronunciation WHERE id=@id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", selectedWordId);
                command.Parameters.AddWithValue("@word", WordTextBox.Text);
                command.Parameters.AddWithValue("@translation", TranslationTextBox.Text);
                command.Parameters.AddWithValue("@pronunciation", PronunciationTextBox.Text);

                connection.Open();
                command.ExecuteNonQuery();
            }
            LoadData();
            ClearFields();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedWordId == -1)
            {
                MessageBox.Show("Please select a word to delete.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM EnglishWords WHERE id=@id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", selectedWordId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            LoadData();
            ClearFields();
        }

        private void WordsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (WordsDataGrid.SelectedItem is EnglishWord selectedWord)
            {
                selectedWordId = selectedWord.Id;
                WordTextBox.Text = selectedWord.Word;
                TranslationTextBox.Text = selectedWord.Translation;
                PronunciationTextBox.Text = selectedWord.Pronunciation;
            }
        }

        private void ClearFields()
        {
            WordTextBox.Text = string.Empty;
            TranslationTextBox.Text = string.Empty;
            PronunciationTextBox.Text = string.Empty;
            selectedWordId = -1;
        }
    }

    public class EnglishWord
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string Translation { get; set; }
        public string Pronunciation { get; set; }
    }
}
