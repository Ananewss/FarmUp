import liff from liff

export function liffload(liffID) {
    $(document).ready(function () {
        return liffInit(liffID);
    });
}

function liffInit(liffId) {
    liff.init({ liffId: liffId, withLoginOnExternalBrowser: true }).then(() => {
        liff.getProfile().then(function (profile) {
            console.log(profile);
            //$("#lineUserID").text(profile.userId);
            alert(profile.userId);
            return profile.userId
        });
    });
}

