app.service('fileUpload', ['$http', '$q', function ($http, $q) {
    this.uploadFileToUrl = function (loanId, description, file, uploadUrl) {
        var deferred = $q.defer();

        var fd = new FormData();
        fd.append('file', file);
        fd.append('loanId', loanId);
        fd.append('description', description);
        $http.post(uploadUrl, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })
        .success(function (result) {
            var savedDocuments = result.data;
            _documents.splice(pIndex, 1, savedDocuments);
            
            deferred.resolve(savedDocuments);
        })
        .error(function () {
        });

        return deferred.promise;
    }
}]);