using UnityEngine;
using Vuforia;

public class VuforiaInitErrorHandler : MonoBehaviour
{
	
	void Awake ()
	{
		FindObjectOfType<VuforiaAbstractBehaviour> ().RegisterVuforiaInitErrorCallback (OnVuforiaInitError);
	}

	private void OnVuforiaInitError (VuforiaUnity.InitError error)
	{
		if (error != VuforiaUnity.InitError.INIT_SUCCESS) {
			string errorText = GetInitErrorMessage (error);
			Debug.LogError ("Vuforia failed to initialize with the error : " + errorText);
		}
	}

	private string GetInitErrorMessage (VuforiaUnity.InitError errorCode)
	{
		string errorText = "Unknown error code : " + errorCode;
		switch (errorCode) {
		case VuforiaUnity.InitError.INIT_EXTERNAL_DEVICE_NOT_DETECTED:
			errorText =
                    "Failed to initialize Vuforia because this " +
			"device is not docked with required external hardware.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_MISSING_KEY:
			errorText =
                    "Vuforia App Key is missing. \n" +
			"Please get a valid key, by logging into your account at developer.vuforia.com and creating a new project.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_INVALID_KEY:
			errorText =
                    "Vuforia App key is invalid. \n" +
			"Please get a valid key, by logging into your account at developer.vuforia.com and creating a new project.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_TRANSIENT:
			errorText =
                    "Unable to contact server. Please try again later.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_PERMANENT:
			errorText =
                    "No network available. Please make sure you are connected to the internet.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_CANCELED_KEY:
			errorText =
                    "This App license key has been cancelled " +
			"and may no longer be used. Please get a new license key.";
			break;
		case VuforiaUnity.InitError.INIT_LICENSE_ERROR_PRODUCT_TYPE_MISMATCH:
			errorText =
                    "Vuforia App key is not valid for this product. Please get a valid key, " +
			"by logging into your account at developer.vuforia.com and choosing the " +
			"right product type during project creation";
			break;
#if (UNITY_IPHONE || UNITY_IOS)
                case VuforiaUnity.InitError.INIT_NO_CAMERA_ACCESS:
                    errorText = 
                        "Camera Access was denied to this App. \n" + 
                        "When running on iOS8 devices, \n" + 
                        "users must explicitly allow the App to access the camera.\n" + 
                        "To restore camera access on your device, go to: \n" + 
                        "Settings > Privacy > Camera > [This App Name] and switch it ON.";
                    break;
#endif
		case VuforiaUnity.InitError.INIT_DEVICE_NOT_SUPPORTED:
			errorText =
                    "Failed to initialize Vuforia because this device is not " +
			"supported.";
			break;
		case VuforiaUnity.InitError.INIT_ERROR:
			errorText = "Failed to initialize Vuforia.";
			break;
		}
		return errorText;
	}

}
