namespace SFA.DAS.AssessorService.ExternalApi.Client.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class CoalesceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ChangeType(values?.FirstOrDefault(v => v != null), targetType);
            }
            catch
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
