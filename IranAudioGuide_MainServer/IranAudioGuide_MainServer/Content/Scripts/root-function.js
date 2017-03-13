var respondstatus =
      {
          success: 0,
          invalidInput: 1,
          ivalidCordinates: 2,
          invalidFileFormat: 3,
          unknownError: 4,
          dbError: 5,
          invalidId: 6,
          forignKeyError: 7
      }

var _qevents = _qevents || [];

(function () {
    var elem = document.createElement('script');
    elem.src = (document.location.protocol == "https:" ? "https://secure" : "http://edge") + ".quantserve.com/quant.js";
    elem.async = true;
    elem.type = "text/javascript";
    var scpt = document.getElementsByTagName('script')[0];
    scpt.parentNode.insertBefore(elem, scpt);
})();

_qevents.push({
    qacct: "p-W5MYZqNRc9gad"
});