/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Facebook.Unity.Arcade
{
    using System;
    using System.Collections.Generic;
    using FacebookGames;
    using FacebookPlatformServiceClient;

    internal class ArcadeWrapper : IArcadeWrapper
    {
        private const string PipeErrorMessage = @"Pipe name not passed to application on start.
 Make sure you are running inside the facebook games client.";

        private FacebookNamedPipeClient clientPipe;
        private ArcadeFacebookGameObject facebookGameObject;

        public ArcadeWrapper()
        {
            string pipeName;
            Utilities.CommandLineArguments.TryGetValue("/pn", out pipeName);
            if (pipeName == null)
            {
                throw new InvalidOperationException(ArcadeWrapper.PipeErrorMessage);
            }

            this.clientPipe = new FacebookNamedPipeClient(pipeName);
            this.facebookGameObject = ComponentFactory.GetComponent<ArcadeFacebookGameObject>();
        }

        public IDictionary<string, object> PipeResponse
        {
            get
            {
                PipePacketResponse response = this.clientPipe.PipeResponse;
                if (response == null)
                {
                    return null;
                }

                return response.ToDictionary();
            }

            set
            {
                if (value == null)
                {
                    this.clientPipe.PipeResponse = null;
                    return;
                }

                throw new NotSupportedException("Can only set pipe response to null");
            }
        }

        public void DoLoginRequest(
            string appID,
            string permissions,
            string callbackId,
            ArcadeFacebook.OnComplete completeDelegate)
        {
            var request = new LoginRequest(
                appID,
                permissions);
            this.HandleRequest<LoginRequest, LoginResponse>(
                request,
                callbackId,
                completeDelegate);
        }

        public void DoPayRequest(
            string appId,
            string method,
            string action,
            string product,
            string productId,
            string quantity,
            string quantityMin,
            string quantityMax,
            string requestId,
            string pricepointId,
            string testCurrency,
            string callbackId,
            ArcadeFacebook.OnComplete completeDelegate)
        {
            var request = new PayRequest(
                appId,
                method,
                action,
                product,
                productId,
                quantity,
                quantityMin,
                quantityMax,
                requestId,
                pricepointId,
                testCurrency);
            this.HandleRequest<PayRequest, PayResponse>(
                request,
                callbackId,
                completeDelegate);
        }

        public void DoFeedShareRequest(
            string appId,
            string toId,
            string link,
            string linkName,
            string linkCaption,
            string linkDescription,
            string pictureLink,
            string mediaSource,
            string callbackId,
            ArcadeFacebook.OnComplete completeDelegate)
        {
            var request = new FeedShareRequest(
                appId,
                toId,
                link,
                linkName,
                linkCaption,
                linkDescription,
                pictureLink,
                mediaSource);
            this.HandleRequest<FeedShareRequest, FeedShareResponse>(
                request,
                callbackId,
                completeDelegate);
        }

        public void DoAppRequestRequest(
            string appId,
            string message,
            string actionType,
            string objectId,
            string to,
            string filters,
            string excludeIDs,
            string maxRecipients,
            string data,
            string title,
            string callbackId,
            ArcadeFacebook.OnComplete completeDelegate)
        {
            var request = new AppRequestRequest(
                appId,
                message,
                actionType,
                objectId,
                to,
                filters,
                excludeIDs,
                maxRecipients,
                data,
                title);
            this.HandleRequest<AppRequestRequest, AppRequestResponse>(
                request,
                callbackId,
                completeDelegate);
        }

        public void SendRequest<T>(T request)
            where T : PipePacketResponse
        {
            this.clientPipe.SendRequest<T>(request);
            return;
        }

        private void HandleRequest<T, R>(
            T request,
            string callbackId,
            ArcadeFacebook.OnComplete completeDelegate)
            where T : PipePacketRequest
            where R : PipePacketResponse
        {
            this.clientPipe.SendRequest<R>(request);
            this.facebookGameObject.WaitForResponse(
                completeDelegate,
                callbackId);
        }
    }
}
