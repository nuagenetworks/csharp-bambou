/*
  Copyright (c) 2017, Nokia
  All rights reserved.

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are met:
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
      * Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.
      * Neither the name of the copyright holder nor the names of its contributors
        may be used to endorse or promote products derived from this software without
        specific prior written permission.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;

namespace net.nuagenetworks.bambou.operation
{
    public interface RestSessionOperations {

        void start();

        void reset();

        void fetch(RestObject restObj);

        void save(RestObject restObj);

        void save(RestObject restObj, int responseChoice);

        void delete(RestObject restObj);

        void delete(RestObject restObj, int responseChoice);

        void createChild(RestObject restObj, RestObject childRestObj);

        void createChild(RestObject restObj, RestObject childRestObj, int responseChoice, bool commit);

        void instantiateChild(RestObject restObj, RestObject childRestObj, RestObject fromTemplate);

        void instantiateChild(RestObject restObj, RestObject childRestObj, RestObject fromTemplate, int responseChoice, bool commit);

        void assign(RestObject restObj, List<RestObject> childRestObjs);

        void assign(RestObject restObj, List<RestObject> childRestObjs, bool commit);

        List<T> get<T>(RestFetcher<T> fetcher) where T:RestObject;

        List<T> fetch<T>(RestFetcher<T> fetcher) where T : RestObject;

        T getFirst<T>(RestFetcher<T> fetcher) where T : RestObject;

        int count<T>(RestFetcher<T> fetcher) where T : RestObject;

        List<T> get<T>(RestFetcher<T> fetcher, String filter, String orderBy, String[] groupBy, int page, int pageSize, String queryParameters, bool commit) where T : RestObject;

        List<T> fetch<T>(RestFetcher<T> fetcher, String filter, String orderBy, String[] groupBy, int page, int pageSize, String queryParameters, bool commit) where T : RestObject;

        T getFirst<T>(RestFetcher<T> fetcher, String filter, String orderBy, String[] groupBy, int page, int pageSize, String queryParameters, bool commit) where T : RestObject;

        int count<T>(RestFetcher<T> fetcher, String filter, String orderBy, String[] groupBy, int page, int pageSize, String queryParameters, bool commit) where T : RestObject;
    }
}