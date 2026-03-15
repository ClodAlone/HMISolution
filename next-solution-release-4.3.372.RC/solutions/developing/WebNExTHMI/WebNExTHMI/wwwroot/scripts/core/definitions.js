const WEBHMIAPP = "WebHMIApp";
const ROOT = '#root';
const ROOTAppBars = '#rootAppBars';
const ROOTAppBarsID = 'rootAppBars';
const ROOTPOPUP = 'rootPopup';
const MAINCONTROLLER = 'MainController';

const syscomponentsPath = './scripts/syscomponents/';
const componentsPath = './scripts/components/';
const toolboxPath = './scripts/toolbox/';
const imageFolder = './images/projectData/';
const documentsFolder = './documents/';

const dataparameters = 'data-parameters';
const popupparameters = 'popup-parameters';

const onloadedHtmlOnDemand = 'onloadedHtmlOnDemand';
const onError = 'onError';
const onindexedDBReady = 'onindexedDBReady';
const onscreenHashReady = 'onscreenHashReady';
const onPopupSizeRequest = 'onPopupSizeRequest';
const onPopupResize = 'onPopupResize';
const onViewClosed = 'onViewClosed';
const onPopupCloseRequest = 'onPopupCloseRequest';
const onMovePopup = 'onMovePopupRequest';
const onDataChanged = 'onDataChanged';
const onActiveLanguageChanged = 'onActiveLanguageChanged';
const onUpdateScreenSettings = 'onUpdateScreenSettings';
const onSRNewCommand = 'onSRNewCommand';
const disabledControlOpacity = 0.5;

const defShadingColor = "rgba(0,0,0,0.4)";

const hideEvent = 'hide';
const hidingEvent = 'hiding';
const contentReady = 'contentReady';

const disabledClass = 'disabled';
const hiddenClass = 'hidden';
const disabledClassEvent = 'disabledClassEvent';
const animationHiddenClass = 'animationHidden';

const maxBackHistoryCount = 20;
const minLegendHeight = 50;
const mouseMoveThrottlingMS = 30;

const refreshRateMS = {
    low: 100,
    mid: 50,
    high: 10
};

const concatTranslationsRegexp = new RegExp('{(.*?)}', 'g');
const invariantNumberDecimalSeparator = '.';
const emptyGuID = "00000000-0000-0000-0000-000000000000";