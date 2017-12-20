using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LyricsManager.MVVM
{
    /// <summary>
    ///     Basisklasse für ein ViewModel. Stellt einfache Methoden zum Setzen der Properties bereit und implementiert
    ///     INotifyPropertyChanged.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Setzt den Wert einer Property und löst PropertyChanged aus.
        /// </summary>
        /// <typeparam name="T">Typ des Wertes</typeparam>
        /// <param name="field">Klassenvariable, in der der Wert gespeichert wird</param>
        /// <param name="value">Neuer Wert</param>
        /// <param name="propertyName">Name der Property</param>
        protected virtual void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if ((object)field == (object)value)
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
