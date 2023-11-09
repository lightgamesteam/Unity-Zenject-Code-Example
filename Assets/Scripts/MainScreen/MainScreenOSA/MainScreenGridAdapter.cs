/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.DataHelpers;
using TDL.Views;
using TDL.Models;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids
{
	// There is 1 important callback you need to implement, apart from Start(): UpdateCellViewsHolder()
	// See explanations below
	public class MainScreenGridAdapter : GridAdapter<GridParams, AssetGridItemViewsHolder>
	{
		[SerializeField] ViewBase zenjectView;

		private List<ClientAssetModel> contentModelItems;

		public bool freezeContentEndEdgeOnCountChange;

		private Texture2D _whiteTex;

		public SimpleDataHelper<ClientAssetModel> Data { get; private set; }


		#region GridAdapter implementation
		protected override void Start()
		{
			Data ??= new SimpleDataHelper<ClientAssetModel>(this);
			
			_whiteTex = Texture2D.whiteTexture;
			Data = new SimpleDataHelper<ClientAssetModel>(this);
			base.Start();
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateCellViewsHolder(AssetGridItemViewsHolder newOrRecycled)
		{
			// In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
			// index of item that should be represented by this views holder. You'll use this index
			// to retrieve the model from your data set
			
			var content = newOrRecycled.Content;
			var model = Data[newOrRecycled.ItemIndex];
			
			content.SetThumbnail(Texture2D.whiteTexture);

			content.Init(zenjectView.SignalProperty);
			zenjectView.SignalProperty.Fire(new MainScreenOsaInitializeAssetSignal(content, model));
		}

		// This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
		// especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
		// download request, if it's still in progress when the item goes out of the viewport.
		// <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
		// *For the method's full description check the base implementation
		/*protected override void OnBeforeRecycleOrDisableCellViewsHolder(AssetGridItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			//We should check if the current thumbnail should be used for the correct view.
			//So we get a new element and "lock" it

			base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}*/
		
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			Data ??= new SimpleDataHelper<ClientAssetModel>(this);
			
			_CellsCount = Data.Count;
			base.Refresh(false, keepVelocity);
		}
		
		#endregion

		public void SetItems(IList<ClientAssetModel> items)
		{
			Data ??= new SimpleDataHelper<ClientAssetModel>(this);
			Data.ResetItems(items);
			Data.NotifyListChangedExternally();
			Refresh();
		}
	}

	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	// The cell views holder should have a single child (usually named "Views"), which contains the actual 
	// UI elements. A cell's root is never disabled - when a cell is removed, only its "views" GameObject will be disabled
	public class AssetGridItemViewsHolder : CellViewsHolder
	{
		private WebAssetItemView content;

		public WebAssetItemView Content
		{
			get
			{
				if (content == null)
				{
					content = views.GetComponentInChildren<WebAssetItemView>();
				}
				return content;
			}
		}
	}
}
