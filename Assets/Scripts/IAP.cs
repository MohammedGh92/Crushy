using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAP : MonoBehaviour,IStoreListener
{
	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;
	public static string PRODUCT_1000_COINS = "coins1000";
	public static string PRODUCT_2000_COINS = "coins2000";
	public static string PRODUCT_5000_COINS = "coins5000";
	private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

	private void Start ()
	{
		// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null) {
			// Begin to configure our connection to Purchasing
			InitializePurchasing ();
		}
	}

	public void InitializePurchasing ()
	{
		// If we have already connected to Purchasing ...
		if (IsInitialized ()) {
			// ... we are done here.
			return;
		}

		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
		builder.AddProduct (PRODUCT_1000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_2000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_5000_COINS, ProductType.Consumable);
		UnityPurchasing.Initialize (this, builder);
	}


	private bool IsInitialized ()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void BuyCoins(int CoinsID){
		if (CoinsID == 0)
			BuyProductID (PRODUCT_1000_COINS);
		else if (CoinsID == 1)
			BuyProductID (PRODUCT_2000_COINS);
		else
			BuyProductID (PRODUCT_5000_COINS);
	}

	void BuyProductID (string productId)
	{
		// If Purchasing has been initialized ...
		if (IsInitialized ()) {
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = m_StoreController.products.WithID (productId);

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if (product != null && product.availableToPurchase) {
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				m_StoreController.InitiatePurchase (product);
			}
					// Otherwise ...
					else {
				// ... report the product look-up failure situation  
				Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
				// Otherwise ...
				else {
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			PopMSG.Instance.ShowPopMSG("Not Available");
			Debug.Log ("BuyProductID FAIL. Not initialized.");
		}
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
	}
	public void OnInitializeFailed (InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
	}
	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_1000_COINS, StringComparison.Ordinal)) {
			Debug.Log (string.Format ("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			GiftsShowerManager.Instance.GiveACoinsGift (2);
		}else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_2000_COINS, StringComparison.Ordinal)) {
			Debug.Log (string.Format ("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			GiftsShowerManager.Instance.GiveACoinsGift (3);
		}
		else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_5000_COINS, StringComparison.Ordinal)) {
			Debug.Log (string.Format ("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			GiftsShowerManager.Instance.GiveACoinsGift (4);
		}
		else {
			Debug.Log (string.Format ("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}

		// Return a flag indicating whether this product has completely been received, or if the application needs 
		// to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
		// saving purchased products to the cloud, and when that save is delayed. 
		return PurchaseProcessingResult.Complete;
	}
	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.
		Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}
}
