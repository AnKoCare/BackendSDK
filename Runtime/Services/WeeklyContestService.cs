using System;
using System.Collections;
using System.Collections.Generic;
using GameBackendModule.Models;

namespace GameBackendModule.Services
{
    public interface IWeeklyContestService
    {
        /// <summary>GET /weekly-contest/status — Bearer JWT.</summary>
        IEnumerator GetStatus(
            Action<ApiResponse<WeeklyContestStatusResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>POST /weekly-contest/claim — Bearer JWT. <paramref name="ackWeekId"/> = tuần frozen cần ack (sau cheat).</summary>
        IEnumerator Claim(
            string ackWeekId,
            Action<ApiResponse<WeeklyContestClaimResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>POST /weekly-contest/add-score — Bearer JWT.</summary>
        IEnumerator AddScore(
            WeeklyContestAddScoreRequest request,
            Action<ApiResponse<WeeklyContestAddScoreResponse>> onSuccess,
            Action<ErrorResponse> onError);

        /// <summary>POST /weekly-contest/cheat/end-week [DEV]. <paramref name="cheatKeyHeader"/> chỉ khi server production.</summary>
        IEnumerator CheatEndWeek(
            string cheatKeyHeader,
            Action<ApiResponse<WeeklyContestCheatEndWeekResponse>> onSuccess,
            Action<ErrorResponse> onError);
    }

    public class WeeklyContestService : IWeeklyContestService
    {
        private readonly IApiClient apiClient;

        public WeeklyContestService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IEnumerator GetStatus(
            Action<ApiResponse<WeeklyContestStatusResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return apiClient.Get<WeeklyContestStatusResponse>(
                ApiConstants.WEEKLY_CONTEST_STATUS_ENDPOINT,
                onSuccess,
                onError);
        }

        public IEnumerator Claim(
            string ackWeekId,
            Action<ApiResponse<WeeklyContestClaimResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            object body = string.IsNullOrEmpty(ackWeekId)
                ? new EmptyBody()
                : new WeeklyContestClaimRequest { ackWeekId = ackWeekId };

            yield return apiClient.Post<WeeklyContestClaimResponse>(
                ApiConstants.WEEKLY_CONTEST_CLAIM_ENDPOINT,
                body,
                onSuccess,
                onError);
        }

        public IEnumerator AddScore(
            WeeklyContestAddScoreRequest request,
            Action<ApiResponse<WeeklyContestAddScoreResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            yield return apiClient.Post<WeeklyContestAddScoreResponse>(
                ApiConstants.WEEKLY_CONTEST_ADD_SCORE_ENDPOINT,
                request,
                onSuccess,
                onError);
        }

        public IEnumerator CheatEndWeek(
            string cheatKeyHeader,
            Action<ApiResponse<WeeklyContestCheatEndWeekResponse>> onSuccess,
            Action<ErrorResponse> onError)
        {
            IReadOnlyDictionary<string, string> headers = null;
            if (!string.IsNullOrEmpty(cheatKeyHeader))
            {
                headers = new Dictionary<string, string>
                {
                    { ApiConstants.WEEKLY_CONTEST_CHEAT_KEY_HEADER, cheatKeyHeader },
                };
            }

            yield return apiClient.Post<WeeklyContestCheatEndWeekResponse>(
                ApiConstants.WEEKLY_CONTEST_CHEAT_END_WEEK_ENDPOINT,
                new EmptyBody(),
                headers,
                onSuccess,
                onError);
        }
    }
}
