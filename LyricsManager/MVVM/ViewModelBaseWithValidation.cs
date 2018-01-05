using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LyricsManager.MVVM
{
    /// <summary>
    ///     Basisklasse für ein ViewModel mit Validierung.
    ///     Implementiert INotifyDataErrorInfo
    /// </summary>
    public class ViewModelBaseWithValidation : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        ///     Fügt einen Fehler hinzu falls dieser noch nicht existiert. Das ErrorsChanged-Event wird ausgelöst.
        /// </summary>
        /// <param name="propertyName">Name der fehlerhaften Property</param>
        /// <param name="error">Fehlermeldung</param>
        public void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        ///     Informiert, dass die Fehler neu überprüft werden sollen. EventHAndler ErrorsChanged wird aufgerufen.
        /// </summary>
        /// <param name="propertyName">Name der Property</param>
        public void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Entfernt einen Fehler und löst das ErrorsChanged-Event aus.
        /// </summary>
        /// <param name="propertyName">Name der fehlerhaften Property</param>
        /// <param name="error">Fehlermeldung</param>
        public void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) &&
                _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        ///     Löscht alle Fehler einer Property.
        /// </summary>
        /// <param name="propertyName">Name der Property</param>
        public void RemoveAllErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                var count = _errors[propertyName].RemoveAll(s => true);
                if (count > 0)
                    RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        ///     Setzt den Wert einer Property und löst PropertyChanged aus. Es kann eine Methode zur Validierung des Wertes
        ///     mitgegeben werden.
        /// </summary>
        /// <typeparam name="T">Typ des Wertes</typeparam>
        /// <param name="validation">
        ///     Funktion die prüft, ob der Wert gültig ist. Die Funktion muss Fehler mit <see cref="AddError" />
        ///     und <see cref="RemoveError" /> setzen bzw. entfernen. Als Parameter bekommt die Funktion den Namen der Property
        ///     übergeben.
        /// </param>
        /// <param name="field">Klassenvariable, in der der Wert gespeichert wird</param>
        /// <param name="value">Neuer Wert</param>
        /// <param name="propertyName">Name der Property</param>
        protected virtual void Set<T>(Func<T, string, bool> validation, ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if ((object)field == (object)value)
                return;
            validation?.Invoke(value, propertyName);
            field = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Setzt bzw. entfernt einen Fehler für eine Property abhängig von einer Bedingung.
        /// </summary>
        /// <param name="valid">
        ///     Funktion, die die Gültigkeit überprüft. Gibt true zurück, wenn der Wert für die Property gültig
        ///     ist.
        /// </param>
        /// <param name="propertyName">Name der Property</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>Gibt zurück, ob der Wert gültig ist.</returns>
        protected virtual bool SetError(Func<bool> valid, string propertyName, string error)
        {
            if (valid())
            {
                RemoveError(propertyName, error);
                return true;
            }
            AddError(propertyName, error);
            return false;
        }

        #region INotifyDataErrorInfo member

        /// <summary>
        ///     Gibt die Fehler zu einer Property zurück.
        /// </summary>
        /// <param name="propertyName">Name der Property</param>
        /// <returns>Liste der Fehler</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_errors.ContainsKey(propertyName)) return null;
            return _errors[propertyName];
        }

        /// <summary>
        ///     Gibt zurück, ob Fehler vorliegen.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion
    }
}