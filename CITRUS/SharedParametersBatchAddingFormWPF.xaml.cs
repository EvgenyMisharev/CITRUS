using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CITRUS
{
    /// <summary>
    /// Логика взаимодействия для SharedParametersBatchAddingFormWPF.xaml
    /// </summary>
    public partial class SharedParametersBatchAddingFormWPF : Window
    {
        public string AddParametersSelectedOption;
        public string FilePath;
        DefinitionGroups SharedParametersGroups;
        ObservableCollection<KeyValuePair<string, BuiltInParameterGroup>> BuiltInParameterGroupKeyValuePairs;
        public ObservableCollection<SharedParametersBatchAddingItem> SharedParametersBatchAddingItemsList;

        public SharedParametersBatchAddingFormWPF(DefinitionGroups sharedParametersGroups, ObservableCollection<KeyValuePair<string, BuiltInParameterGroup>> builtInParameterGroupKeyValuePairs)
        {
            InitializeComponent();
            //Создание коллекции для вывода в комбобокс выбора группирования
            BuiltInParameterGroupKeyValuePairs = builtInParameterGroupKeyValuePairs;
            comboBox_GroupingParameters.ItemsSource = BuiltInParameterGroupKeyValuePairs;
            if (BuiltInParameterGroupKeyValuePairs.Where(kVP => kVP.Key.Equals("Прочее")).Count() != 0)
            {
                comboBox_GroupingParameters.SelectedItem = comboBox_GroupingParameters.Items.GetItemAt(BuiltInParameterGroupKeyValuePairs.IndexOf(BuiltInParameterGroupKeyValuePairs.FirstOrDefault(kVP => kVP.Key == "Прочее")));
            }
            comboBox_GroupingParameters.DisplayMemberPath = "Key";

            //Группа параметров из ФОП
            SharedParametersGroups = sharedParametersGroups;
            listBox_SharedParametersGroups.ItemsSource = SharedParametersGroups;
            listBox_SharedParametersGroups.DisplayMemberPath = "Name";
            listBox_SharedParametersGroups.SelectedItem = listBox_SharedParametersGroups.Items.GetItemAt(0);

            //Создание списка выбранных параметров
            SharedParametersBatchAddingItemsList = new ObservableCollection<SharedParametersBatchAddingItem>();
            dataGrid_SelectedParametersGroup.ItemsSource = SharedParametersBatchAddingItemsList;
            dataGridComboBoxColumnGroup.ItemsSource = BuiltInParameterGroupKeyValuePairs;
            dataGridComboBoxColumnGroup.DisplayMemberPath = "Key";
            dataGridComboBoxColumnGroup.SelectedValuePath = "Value";
        }

        //Показ списка параметров при выборе группы параметров
        private void SharedParametersGroupOnSelected(object sender, SelectionChangedEventArgs args)
        {
            DefinitionGroup definitionGroup = ((sender as ListBox).SelectedItem as DefinitionGroup);
            Definitions SharedParametersSelectedDefinitions = SharedParametersGroups.get_Item(definitionGroup.Name).Definitions;
            listBox_SharedParameters.ItemsSource = SharedParametersSelectedDefinitions;
            listBox_SharedParameters.DisplayMemberPath = "Name";
        }

        //Изменение принципа здания параметров (семейство, семейства, папка)
        private void radioButton_AddParametersGroupChecked(object sender, RoutedEventArgs e)
        {
            string ActionSelectionButtonName = (this.groupBox_AddParameters.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;
            if(ActionSelectionButtonName == "radioButton_ActiveFamily")
            {
                btn_FolderBrowserDialog.IsEnabled = false;
                richTextBox_FamiliesFolderPath.IsEnabled = false;
            }
            else if (ActionSelectionButtonName == "radioButton_AllOpenFamilies")
            {
                btn_FolderBrowserDialog.IsEnabled = false;
                richTextBox_FamiliesFolderPath.IsEnabled = false;
            }
            else if (ActionSelectionButtonName == "radioButton_FamiliesInSelectedFolder")
            {
                btn_FolderBrowserDialog.IsEnabled = true;
                richTextBox_FamiliesFolderPath.IsEnabled = true;
            }
        }

        //Получение папки с файлами семейств
        private void btn_FolderBrowserDialog_Click(object sender, EventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dialog.SelectedPath;
                richTextBox_FamiliesFolderPath.Document.Blocks.Clear();
                richTextBox_FamiliesFolderPath.AppendText(FilePath);
            }
        }

        //Добавление выбранного параметра в список двойным щелчком
        private void listBox_SharedParameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Создание элемента выбора и заполнение свойств
            SharedParametersBatchAddingItem sharedParametersBatchAddingItem = new SharedParametersBatchAddingItem();
            sharedParametersBatchAddingItem.ExternalDefinitionParam = listBox_SharedParameters.SelectedItem as ExternalDefinition;
            sharedParametersBatchAddingItem.ExternalDefinitionParamGuid = (listBox_SharedParameters.SelectedItem as ExternalDefinition).GUID;
            string radioButtonParameterIn = (groupBox_ParameterIn.Content as System.Windows.Controls.Grid).Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked.Value == true).Name;
            if (radioButtonParameterIn == "radioButton_InstanceParameter")
            {
                sharedParametersBatchAddingItem.AddParameterSelectedOptionParam = true;
            }
            else
            {
                sharedParametersBatchAddingItem.AddParameterSelectedOptionParam = false;
            }
            sharedParametersBatchAddingItem.BuiltInParameterGroupParam = (KeyValuePair<string, BuiltInParameterGroup>)comboBox_GroupingParameters.SelectedValue;
            sharedParametersBatchAddingItem.FormulaParam = null;

            //Добавление элемента выбора в список выбранных параметров
            List<SharedParametersBatchAddingItem> sharedParametersItemsTmp = SharedParametersBatchAddingItemsList.Where(p => p.ExternalDefinitionParam == sharedParametersBatchAddingItem.ExternalDefinitionParam).ToList();
            if(sharedParametersItemsTmp.Count == 0)
            {
                SharedParametersBatchAddingItemsList.Add(sharedParametersBatchAddingItem);
            }
        }

        //Удаление выбранного параметра из списка
        private void dataGrid_SelectedParametersGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(SharedParametersBatchAddingItemsList.Count != 0)
            {
                SharedParametersBatchAddingItemsList.Remove(SharedParametersBatchAddingItemsList.Where(i => i.ExternalDefinitionParam == ((sender as DataGrid).SelectedItem as SharedParametersBatchAddingItem).ExternalDefinitionParam).Single());
            }
        }

        //Открыть 
        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new System.Windows.Forms.OpenFileDialog();
            openDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = openDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = openDialog.FileName;
                SharedParametersBatchAddingSettings sharedParametersBatchAddingSettings = new SharedParametersBatchAddingSettings();
                SharedParametersBatchAddingItemsList = sharedParametersBatchAddingSettings.GetSettings(SharedParametersGroups, jsonFilePath);
                dataGrid_SelectedParametersGroup.ItemsSource = SharedParametersBatchAddingItemsList;
            }
        }

        //Сохранить
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = saveDialog.FileName;
                SharedParametersBatchAddingSettings sharedParametersBatchAddingSettings = new SharedParametersBatchAddingSettings();
                sharedParametersBatchAddingSettings.Save(SharedParametersBatchAddingItemsList, jsonFilePath);
            }
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            AddParametersSelectedOption = (groupBox_AddParameters.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;

            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void SharedParametersBatchAddingFormWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                AddParametersSelectedOption = (groupBox_AddParameters.Content as System.Windows.Controls.Grid)
                    .Children.OfType<RadioButton>()
                    .FirstOrDefault(rb => rb.IsChecked.Value == true)
                    .Name;

                this.DialogResult = true;
                this.Close();
            }

            else if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
