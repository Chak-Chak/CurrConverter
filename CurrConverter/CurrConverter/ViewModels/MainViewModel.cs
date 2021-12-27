using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Essentials;

namespace CurrConverter.ViewModels
{
    class MainViewModel: BindableObject
    {
        public HttpClient httpClient;

        public Regex regex = new Regex(@"[0123456789.]");

        public readonly MainPage _page;

        public MainViewModel(MainPage page)
        {
            ValList = new ObservableCollection<Valute>();
            _page = page;
            httpClient = new HttpClient();
            MaxDate = DateTime.Today;
            LoadInfoPreferences();
            //SelectedDate = DateTime.Today;
            //FirstIndex = 10;
            //SecondIndex = 10;
        }

        private void UpdateList(List<Valute> NewList)
        {
            foreach (var element in NewList)
            {
                var inArray = ValList.FirstOrDefault(x => x.ID == element.ID);
                if (inArray != null)
                {
                    inArray.UpdateFields(element);
                }
                else
                {
                    ValList.Add(element);
                }
            }
        }

        private string _firstValue;
        public string FirstValue
        {
            get => _firstValue;
            set
            {
                if (_firstValue != value)
                {
                    _firstValue = value;
                    FirstTextChanged();
                    OnPropertyChanged(nameof(FirstValue));
                }
            }
        }

        private string _secondValue;
        public string SecondValue
        {
            get => _secondValue;
            set
            {
                if (_secondValue != value)
                {
                    _secondValue = value;
                    SecondTextChanged();
                    OnPropertyChanged(nameof(SecondValue));
                }
            }
        }

        private int _firstIndex { get; set; }
        public int FirstIndex
        {
            get => _firstIndex;
            set
            {
                if (_firstIndex != value)
                {
                    _firstIndex = value;
                    if (value != -1) FirstTextChanged();
                    OnPropertyChanged(nameof(FirstIndex));
                }
            }
        }

        private Valute _firstItem { get; set; }
        public Valute FirstItem
        {
            get => _firstItem;
            set
            {
                if (_firstItem != value)
                {
                    _firstItem = value;
                    OnPropertyChanged(nameof(FirstItem));
                }
            }
        }

        private int _secondIndex { get; set; }
        public int SecondIndex
        {
            get => _secondIndex;
            set
            {
                if (_secondIndex != value)
                {
                    _secondIndex = value;
                    if (value != -1) SecondTextChanged();
                    OnPropertyChanged(nameof(SecondIndex));
                }
            }
        }

        public void FirstTextChanged()
        {
            bool result = double.TryParse(FirstValue, out double value);
            
            if (result == true && FirstIndex != -1 && SecondIndex != -1)
            {
                //todo transparent border and calculating
                //_page.FirstTextFrameProp.BorderColor = Color.Transparent;
                
                int firstNominal = ValList[FirstIndex].Nominal;
                double firstValue = ValList[FirstIndex].Value;
                int secondNominal = ValList[SecondIndex].Nominal;
                double secondValue = ValList[SecondIndex].Value;

                double N = firstValue / firstNominal * value;
                SetSecondValue(Math.Round(N / (secondValue / secondNominal), 4).ToString());
            }
            else
            {
                //todo red border and clear second entry
                //_page.FirstTextFrameProp.BorderColor = Color.Red;
                //SecondValue = "";
            }
        }

        public void SecondTextChanged()
        {
            bool result = double.TryParse(SecondValue, out double value);

            if ((result == true) && (FirstIndex != -1) && (SecondIndex != -1))
            {
                //todo transparent border and calculating
                //_page.SecondTextFrameProp.BorderColor = Color.Transparent;
                int firstNominal = ValList[FirstIndex].Nominal;
                double firstValue = ValList[FirstIndex].Value;
                int secondNominal = ValList[SecondIndex].Nominal;
                double secondValue = ValList[SecondIndex].Value;

                double N = firstValue / firstNominal * value;
                SetFirstValue(Math.Round(N / (secondValue / secondNominal), 4).ToString());
            }
            else
            {
                //todo red border and clear first entry
                //_page.SecondTextFrameProp.BorderColor = Color.Red;
                //FirstValue = "";
            }
        }

        private void SetFirstIndex(int value)
        {
            _firstIndex = value;
            OnPropertyChanged(nameof(FirstIndex));
        }

        private void  SetFirstValue(string value)
        {
            _firstValue = value;
            OnPropertyChanged(nameof(FirstValue));
        }

        private void SetSecondValue(string value)
        {
            _secondValue = value;
            OnPropertyChanged(nameof(SecondValue));
        }

        public List<Valute> GetValues()
        {
            try
            {
                string selDate = SelectedDate.ToString("yyyy/MM/dd");
                var responseResult = httpClient.GetStringAsync($"https://www.cbr-xml-daily.ru/archive/{selDate}/daily_json.js")
                    .Result;
                var jsonResult = JObject.Parse(responseResult);
                var arrayResult = JsonConvert.DeserializeObject<Dictionary<string, Valute>>(jsonResult["Valute"].ToString());
                //return arrayResult.Select(x => x.Value).ToArray();
                
                return arrayResult.Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SelectedDate = SelectedDate.AddDays(-1);
                //ValList = GetValues();
                //return Array.Empty<Valute>();
                return GetValues();
            }
        }
        private ObservableCollection<Valute> _valList { get; set; }
        public ObservableCollection<Valute> ValList
        {
            get => _valList;
            set
            {
                _valList = value;
                OnPropertyChanged(nameof(ValList));
            }
        }
        private DateTime _maxDate { get; set; }
        public DateTime MaxDate
        {
            get => _maxDate;
            set
            {
                if (_maxDate != value)
                {
                    _maxDate = value;
                    OnPropertyChanged(nameof(MaxDate));
                }
            }
        }
        private DateTime _minDate { get; set; }
        public DateTime MinDate
        {
            get => _minDate;
            set
            {
                if (_minDate != value)
                {
                    _minDate = value;
                    OnPropertyChanged(nameof(MinDate));
                }
            }
        }
        private DateTime _selectedDate { get; set; }
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    List<Valute> tempList = GetValues();
                    UpdateList(tempList);
                    FirstTextChanged();
                    OnPropertyChanged(nameof(FirstValue));
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }
        
        public void SaveInfoPreferences()
        {
            Preferences.Set("ValList", JsonConvert.SerializeObject(ValList));
            Preferences.Set("Date", SelectedDate);
            Preferences.Set("FirstIndex", FirstIndex);
            Preferences.Set("SecondIndex", SecondIndex);
            Preferences.Set("FirstValue", FirstValue);
            Preferences.Set("SecondValue", SecondValue);
        }

        public void LoadInfoPreferences()
        {
            var itemJson = Preferences.Get("ValList", "[]");
            ValList = JsonConvert.DeserializeObject<ObservableCollection<Valute>>(itemJson);
            var dateJson = Preferences.Get("Date", DateTime.Today);
            SelectedDate = dateJson;
            var firstValueJson = Preferences.Get("FirstValue", "0");
            SetFirstValue(firstValueJson);
            var secondValueJson = Preferences.Get("SecondValue", "0");
            SetSecondValue(secondValueJson);
            var firstIndexJson = Preferences.Get("FirstIndex", -1);
            SetFirstIndex(firstIndexJson);
            var secondIndexJson = Preferences.Get("SecondIndex", -1);
            SecondIndex = secondIndexJson;
        }

        public class Valute
        {
            public string ID { get; set; }
            public string NumCode { get; set; }
            public string CharCode { get; set; }
            public int Nominal { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public double Previous { get; set; }

            public void UpdateFields(Valute valute)
            {
                NumCode = valute.NumCode;
                CharCode = valute.CharCode;
                Nominal = valute.Nominal;
                Name = valute.Name;
                Value = valute.Value;
                Previous = valute.Previous;
            }
        }
    }
}
