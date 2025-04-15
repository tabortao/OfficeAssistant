using System;
using System.Collections.Generic;  // 用于 EqualityComparer
using System.ComponentModel;       // 用于属性更改通知接口
using System.Runtime.CompilerServices;  // 用于 CallerMemberName 特性
using System.Threading.Tasks;      // 用于异步操作

namespace OfficeAssistant.ViewModels
{
    /// <summary>
    /// 视图模型基类，实现了属性更改通知机制
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性更改事件，当属性值发生变化时触发
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 触发属性更改事件的保护方法
        /// </summary>
        /// <param name="propertyName">发生更改的属性名称，默认使用调用者的成员名</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 设置字段值并在值发生变化时触发属性更改事件
        /// </summary>
        /// <typeparam name="T">字段类型</typeparam>
        /// <param name="field">字段的引用</param>
        /// <param name="value">要设置的新值</param>
        /// <param name="propertyName">属性名称，默认使用调用者的成员名</param>
        /// <returns>如果值发生了变化返回true，否则返回false</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // 如果新值与当前值相等，则不进行更改
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            // 更新字段值
            field = value;
            // 触发属性更改事件
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 显示临时消息，在指定时间后自动清除
        /// </summary>
        /// <param name="message">要显示的消息内容</param>
        /// <param name="setMessage">设置消息的委托方法</param>
        /// <param name="duration">消息显示持续时间（毫秒），默认5000毫秒</param>
        protected static async Task ShowTemporaryMessage(string message, Action<string> setMessage, int duration = 5000)
        {
            // 显示消息
            setMessage(message);
            // 等待指定时间
            await Task.Delay(duration);
            // 清除消息
            setMessage("");
        }
    }
}