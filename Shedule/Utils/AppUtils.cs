using LearningProcessesAPIClient.exceptions;
using Shedule.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Shedule.Utils
{
    public class AppUtils
    {

        public static async Task ProcessClientLibraryRequest(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (LibraryErrorException e)
            {
                switch (e.ErrorCode)
                {
                    case LibraryErrorException.ErrorCodes.AUTH_ERROR:
                        if(MessageBox.Show("Укажите корректные логин и пароль в настройках!\n\nПерейти в настройки?", "Ошибка авторизации!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MainWindow.Instance.MainFrame.NavigationService.Navigate(new Settings());
                        }
                        
                        break;
                    case LibraryErrorException.ErrorCodes.ENDPOINT_ERROR:
                        if(MessageBox.Show("Сервер не указан или недоступен!\nУкажите корректный адрес и порт в настройках или обратитесь к администратору\n\nПерейти в настройки?", "Ошибка соединения!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            MainWindow.Instance.MainFrame.NavigationService.Navigate(new Settings());
                        }
                        ;
                        break;
                    case LibraryErrorException.ErrorCodes.UNKNOWN_SERVER_ERROR:
                        MessageBox.Show("При попытке соединения с сервером возникла следующая ошибка:\n" + e.Message, "Серверная ошибка!",MessageBoxButton.OK,MessageBoxImage.Error);
                        break;
                }
                throw;
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show("При попытке соединения с сервером возникла следующая ошибка:\n" + e.Message, "Серверная ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (RequestErrorException e)
            {
                MessageBox.Show("При отправке запрроса произошла следующая ошибка:\n" + e.Message, "Ошибка данных!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show("При выполнении программы возникла следующая ошибка:\n" + e.Message, "Ошибка приложения!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            
        }

    }
}
