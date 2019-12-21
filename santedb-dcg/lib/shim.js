/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justin
 * Date: 2018-7-4
 */

// SanteDB Self-Hosted SHIM

__SanteDBAppService.GetStatus = function () {
    return '[ "Dummy Status", 0 ]';
}

__SanteDBAppService.ShowToast = function (string) {
    console.info("TOAST: " + string);
}

__SanteDBAppService.GetOnlineState = function () {
    return true;
}


__SanteDBAppService.IsAdminAvailable = function () {
    return true;
}

__SanteDBAppService.IsClinicalAvailable = function () {
    return true;
}


__SanteDBAppService.BarcodeScan = function () {
    return SanteDBApplicationService.NewGuid().substring(0, 8);
}

__SanteDBAppService.Close = function () {
    alert("You need to restart the service for the changes to take effect");
    window.close();
}

__SanteDBAppService.GetLocale = function () {
    return (navigator.language || navigator.userLanguage).substring(0, 2);
}

__SanteDBAppService.NewGuid = function () {
    var retVal = "";
    $.ajax({
        url: "/app/Uuid",
        success: function (data) { retVal = data; },
        async: false
    });
    return retVal;
}

__SanteDBAppService.GetMagic = function () {
    return EmptyGuid;
}