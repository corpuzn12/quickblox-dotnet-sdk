﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XamarinForms.QbChat.Repository;
using Quickblox.Sdk.Modules.UsersModule.Models;
using System.Linq;

namespace XamarinForms.QbChat
{
	public partial class GroupChatPage : ContentPage
	{
		private string dialogId;
		int opponentId;
		private bool isLoaded;
		DialogTable dialog;
		List<User> opponentUsers;
		Quickblox.Sdk.Modules.ChatXmppModule.GroupChatManager groupChatManager;


		public GroupChatPage (String dialogId)
		{
			InitializeComponent();
			this.dialogId = dialogId;
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			Database.Instance().UnSubscribeForMessages(OnMessagesChanged);
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (isLoaded)
				return;

			this.busyIndicator.IsVisible = true;

			dialog = Database.Instance().GetDialog(dialogId);
			groupChatManager = App.QbProvider.GetXmppClient ().GetGroupChatManager (dialog.XmppRoomJid, dialogId);


			chatNameLabel.Text = dialog.Name;
			chatPhotoImage.Source = Device.OnPlatform (iOS: ImageSource.FromFile ("ic_user.png"),
				Android: ImageSource.FromFile ("ic_user.png"),
				WinPhone: ImageSource.FromFile ("Images/ic_user.png"));

			opponentUsers = await App.QbProvider.GetUsersByIdsAsync (dialog.OccupantIds);
			if (!string.IsNullOrEmpty (dialog.Photo)) {
				chatPhotoImage.Source = ImageSource.FromUri (new Uri (dialog.Photo));
			}

			List<MessageTable> messages;
			try {
				messages = await App.QbProvider.GetMessagesAsync(dialogId);
			} catch (Exception ex) {
				await App.Current.MainPage.DisplayAlert ("Error", ex.ToString(), "Ok");
				return;
			}

			if (messages != null)
			{
				messages = messages.OrderBy(message => message.DateSent).ToList();

				foreach (var message in messages) {
					if (message.SenderId == App.QbProvider.UserId) {
						message.RecepientFullName = "Me";
					} else {
						var opponentUser = opponentUsers.FirstOrDefault (u => u.Id == message.SenderId);
						if (opponentUser != null) {
							message.RecepientFullName = opponentUser.FullName;
						}
					}

					message.Text = System.Net.WebUtility.UrlDecode (message.Text);
				}

				Database.Instance().SaveAllMessages(dialogId, messages);

				var template = new DataTemplate (typeof(TextCell));
				template.SetBinding (ImageCell.TextProperty, "RecepientFullName");
				template.SetBinding (ImageCell.DetailProperty, "Text");
				listView.ItemTemplate = template;
				var sorted = messages.OrderBy(d => d.DateSent);
				listView.ItemsSource = sorted;

				try {
					if (messages != null && messages.Count > 10)
						listView.ScrollTo (messages [messages.Count - 1], ScrollToPosition.Start, false);
				} catch (Exception ex) {
				}
			}

			Database.Instance().SubscribeForMessages(OnMessagesChanged);

			sendButton.Clicked += SendClicked;

			this.busyIndicator.IsVisible = false;
			this.isLoaded = true;
		}

		private async void SendClicked(object sender, EventArgs e)
		{
			//			if (dialog != null && 
			//				(dialog.DialogType == DialogType.Group ||
			//				 dialog.DialogType == DialogType.PublicGroup)) {
			//				await App.Current.MainPage.DisplayAlert ("Error", "Comming soon. Use private chat for testing.", "Ok");
			//				return;
			//			}

			var message = messageEntry.Text != null ? messageEntry.Text.Trim() : string.Empty;
			if (!string.IsNullOrEmpty(message))
			{
//if (dialog.DialogType == DialogType.Private) {
//				var m = new MessageTable ();
//				m.SenderId = (int)App.QbProvider.UserId;
//				m.Text = message;
//				m.DialogId = dialogId;
//				m.RecepientFullName = "Me";
//				m.DateSent = DateTime.UtcNow.Ticks / 1000;

				dialog.LastMessage = message;
				dialog.LastMessageSent = DateTime.UtcNow;
				Database.Instance ().SaveDialog (dialog);

				try {
					var encodedMessage = System.Net.WebUtility.UrlEncode(message);
					groupChatManager.SendMessage(encodedMessage);
				} catch (Exception ex) {
					await App.Current.MainPage.DisplayAlert ("Error", ex.ToString(), "Ok");
				}

				messageEntry.Text = "";
				//}
			}
		}

		public void OnMessagesChanged()
		{
			var messages = Database.Instance().GetMessages(dialogId);
			var sorted = messages.OrderBy(d => d.DateSent).ToList();
			Device.BeginInvokeOnMainThread (() =>
				{ listView.ItemsSource = sorted;
					try {
						if (messages != null && messages.Count > 10)
							listView.ScrollTo (sorted [sorted.Count - 1], ScrollToPosition.Start, false);
					} catch (Exception ex) {
					}
				}
			);
		}
	}
}

