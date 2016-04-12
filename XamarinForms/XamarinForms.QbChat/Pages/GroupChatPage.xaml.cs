﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XamarinForms.QbChat.Repository;
using Quickblox.Sdk.Modules.UsersModule.Models;
using System.Linq;
using Quickblox.Sdk.GeneralDataModel.Models;
using XamarinForms.QbChat.ViewModels;

namespace XamarinForms.QbChat.Pages
{
	public partial class GroupChatPage : ContentPage
	{
	    private string dialogId;
		private bool isLoaded;
        
		//List<User> opponentUsers;
		//Quickblox.Sdk.Modules.ChatXmppModule.GroupChatManager groupChatManager;


		public GroupChatPage (String dialogId)
		{
			InitializeComponent();

		    this.dialogId = dialogId;
			listView.ItemTapped += (object sender, ItemTappedEventArgs e) => ((ListView)sender).SelectedItem = null;
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			//Database.Instance().UnSubscribeForMessages(OnMessagesChanged);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

            if (isLoaded)
                return;

            this.isLoaded = true;

            var vm = new GroupChatViewModel(dialogId);
		    this.BindingContext = vm;
            vm.OnAppearing();

            //Database.Instance().SubscribeForMessages(OnMessagesChanged);


			//this.busyIndicator.IsVisible = true;

			//var dialog = Database.Instance().GetDialog(dialogId);
			//groupChatManager = App.QbProvider.GetXmppClient ().GetGroupChatManager (dialog.XmppRoomJid, dialogId);

			//chatNameLabel.Text = dialog.Name;
			//chatPhotoImage.Source = Device.OnPlatform (iOS: ImageSource.FromFile ("groupholder.png"),
			//	Android: ImageSource.FromFile ("groupholder.png"),
			//	WinPhone: ImageSource.FromFile ("groupholder.png"));

			//if (!string.IsNullOrEmpty (dialog.Photo)) {
			//	chatPhotoImage.Source = ImageSource.FromUri (new Uri (dialog.Photo));
			//}

			//List<Message> messages;
			//try {
			//	messages = await App.QbProvider.GetMessagesAsync(dialogId);
			//	var userIds = messages.Select( u => u.SenderId).Distinct();
			//	opponentUsers = await App.QbProvider.GetUsersByIdsAsync (string.Join(",", userIds));
			//} catch (Exception ex) {
			//	await App.Current.MainPage.DisplayAlert ("Error", ex.ToString(), "Ok");
			//	return;
			//}

			//if (messages != null)
			//{
			//	List<MessageTable> messageTableList = new List<MessageTable> ();
			//	messages = messages.OrderBy(message => message.DateSent).ToList();
			//	foreach (var message in messages) {
			//		var chatMessage = new MessageTable ();
			//		chatMessage.DateSent = message.DateSent;
			//		chatMessage.SenderId = message.SenderId;
			//		chatMessage.MessageId = message.Id;
			//		if (message.RecipientId.HasValue)
			//			chatMessage.RecepientId = message.RecipientId.Value;
			//		chatMessage.DialogId = message.ChatDialogId;
			//		chatMessage.IsRead = message.Read == 1;

			//		if (chatMessage.SenderId == App.QbProvider.UserId) {
			//			chatMessage.RecepientFullName = "Me";
			//		} else {
			//			var opponentUser = opponentUsers.FirstOrDefault (u => u.Id == message.SenderId);
			//			if (opponentUser != null) {
			//				chatMessage.RecepientFullName = opponentUser.FullName;
			//			}
			//		}

			//		if (message.NotificationType == NotificationTypes.GroupCreate ||
			//		    message.NotificationType == NotificationTypes.GroupUpdate) {
			//			if (message.AddedOccupantsIds.Any ()) {
			//				var userIds = new List<int>(message.AddedOccupantsIds);
			//				userIds.Add (message.SenderId);

			//				var users = await App.QbProvider.GetUsersByIdsAsync (string.Join(",", userIds));

			//				var addedUsers = users.Where (u => u.Id != message.SenderId);
			//				var senderUser = users.First (u => u.Id == message.SenderId);
			//				chatMessage.Text = senderUser.FullName + " added users: " + string.Join (",", addedUsers.Select (u => u.FullName));
			//			} else if (message.DeletedOccupantsIds.Any ()) {
			//				var userIds = new List<int>(message.DeletedOccupantsIds);
			//				var users = await App.QbProvider.GetUsersByIdsAsync (string.Join(",", userIds));
			//				chatMessage.Text = string.Join (",", users.Select (u => u.FullName)) + " left this room";
			//			}
			//		} else {
			//			chatMessage.Text = System.Net.WebUtility.UrlDecode (message.MessageText);
			//		}

			//		messageTableList.Add (chatMessage);
			//	}

			//	Database.Instance().SaveAllMessages(dialogId, messageTableList);
			//}


			//sendButton.Clicked += SendClicked;

			//this.busyIndicator.IsVisible = false;
			//this.isLoaded = true;
		}

		//private async void SendClicked(object sender, EventArgs e)
		//{
		//	var message = messageEntry.Text != null ? messageEntry.Text.Trim() : string.Empty;
		//	if (!string.IsNullOrEmpty(message))
		//	{
		//		var dialog = Database.Instance().GetDialog(dialogId);
		//		dialog.LastMessage = message;
		//		dialog.LastMessageSent = DateTime.UtcNow;
		//		Database.Instance ().SaveDialog (dialog);

		//		try {
		//			var encodedMessage = System.Net.WebUtility.UrlEncode(message);
		//			groupChatManager.SendMessage(encodedMessage);
		//		} catch (Exception ex) {
		//			this.busyIndicator.IsVisible = true;
		//			try {
		//				App.QbProvider.GetXmppClient ().Connect (App.UserId, App.UserPassword);
		//			} catch (Exception ex2) {
		//				App.Current.MainPage.DisplayAlert ("Error", "Please, check your internet connection", "Ok");
		//			}
		//			finally{
		//				this.busyIndicator.IsVisible = false;
		//			}

		//			return;
		//			//await App.Current.MainPage.DisplayAlert ("Error", ex.ToString(), "Ok");
		//		}

		//		messageEntry.Text = "";
		//	}
		//}

		public void ScrollList ()
		{
			var sorted = listView.ItemsSource as List<MessageTable>;
			try {
				if (sorted != null && sorted.Count > 10) {
					listView.ScrollTo (sorted [sorted.Count - 1], ScrollToPosition.End, false);
				}
			}
			catch (Exception ex) {
			}
		}

		//public void OnMessagesChanged()
		//{
		//	var messages = Database.Instance().GetMessages(dialogId);
		//	var sorted = messages.OrderBy(d => d.DateSent).ToList();
		//	Device.BeginInvokeOnMainThread (() =>
		//		{ 
		//			listView.ItemsSource = sorted;
		//			ScrollList ();
		//		}
		//	);
		//}
	}
}

